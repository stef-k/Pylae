using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Forms;
using Pylae.Desktop.Resources;
using Microsoft.Win32;

namespace Pylae.Desktop.Services;

public class IdleLockService : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly CurrentUserService _currentUserService;
    private readonly IServiceProvider _services;
    private readonly int _timeoutMinutes;
    private DateTime _lastInput = DateTime.Now;

    public IdleLockService(int timeoutMinutes, CurrentUserService currentUserService, IServiceProvider services)
    {
        _timeoutMinutes = timeoutMinutes;
        _currentUserService = currentUserService;
        _services = services;
        _timer = new System.Windows.Forms.Timer { Interval = 30_000 };
        _timer.Tick += CheckIdle;
        _timer.Start();
        Application.Idle += (_, _) => _lastInput = DateTime.Now;
        _currentUserService.CurrentUserChanged += _ => _lastInput = DateTime.Now;
        SystemEvents.SessionSwitch += OnSessionSwitch;
    }

    private void CheckIdle(object? sender, EventArgs e)
    {
        if (_timeoutMinutes <= 0)
        {
            return;
        }

        if ((DateTime.Now - _lastInput).TotalMinutes < _timeoutMinutes)
        {
            return;
        }

        _timer.Stop();
        var user = _currentUserService.CurrentUser;
        if (user is null)
        {
            _timer.Start();
            return;
        }

        using var lockForm = _services.GetRequiredService<LockForm>();
        lockForm.SetCurrentUser(user);
        var result = lockForm.ShowDialog();

        switch (result)
        {
            case DialogResult.OK:
                _lastInput = DateTime.Now;
                break;
            case DialogResult.Retry:
                PromptSwitchUser();
                break;
            default:
                _lastInput = DateTime.Now;
                break;
        }

        _timer.Start();
    }

    private void PromptSwitchUser()
    {
        using var loginForm = _services.GetRequiredService<LoginForm>();
        loginForm.PrepareForReauthentication(null);
        var dialogResult = loginForm.ShowDialog();
        if (dialogResult == DialogResult.OK && loginForm.AuthenticatedUser is not null)
        {
            _currentUserService.CurrentUser = loginForm.AuthenticatedUser;
            _lastInput = DateTime.Now;
            return;
        }

        if (dialogResult == DialogResult.Cancel)
        {
            return;
        }

        MessageBox.Show(Strings.Lock_SwitchUserFailed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
        SystemEvents.SessionSwitch -= OnSessionSwitch;
    }

    private void OnSessionSwitch(object? sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionLock)
        {
            _lastInput = DateTime.MinValue;
            CheckIdle(this, EventArgs.Empty);
        }

        if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            _lastInput = DateTime.Now;
        }
    }
}
