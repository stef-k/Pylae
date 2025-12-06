using System.ComponentModel;
using System.Windows.Forms;
using Pylae.Core.Models;

namespace Pylae.Desktop.Helpers;

/// <summary>
/// Helper class for setting up DataGridView virtual mode with Members.
/// Virtual mode only loads visible rows, making it efficient for large datasets.
/// </summary>
public class MembersVirtualModeHelper
{
    private readonly DataGridView _grid;
    private List<Member> _members = [];
    private List<Member> _filteredMembers = [];
    private string _searchFilter = string.Empty;

    // Column definitions for virtual mode
    private static readonly (string Name, string Header, int Width)[] Columns =
    [
        ("MemberNumber", "No.", 60),
        ("FirstName", "First Name", 120),
        ("LastName", "Last Name", 120),
        ("BusinessRank", "Rank", 100),
        ("Office", "Office", 100),
        ("MemberTypeName", "Type", 100),
        ("IsActive", "Active", 60),
    ];

    public MembersVirtualModeHelper(DataGridView grid)
    {
        _grid = grid;
    }

    /// <summary>
    /// Sets up the grid for virtual mode operation.
    /// </summary>
    public void Setup()
    {
        _grid.VirtualMode = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.ReadOnly = true;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = true;
        _grid.AutoGenerateColumns = false;

        // Add columns
        _grid.Columns.Clear();
        foreach (var (name, header, width) in Columns)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                Width = width,
                DataPropertyName = name
            });
        }

        // Wire up virtual mode events
        _grid.CellValueNeeded += OnCellValueNeeded;
    }

    /// <summary>
    /// Loads data into the virtual mode grid.
    /// </summary>
    public void LoadData(IEnumerable<Member> members)
    {
        _members = members.ToList();
        ApplyFilter();
    }

    /// <summary>
    /// Applies search filter to the data.
    /// </summary>
    public void SetFilter(string filter)
    {
        _searchFilter = filter?.Trim() ?? string.Empty;
        ApplyFilter();
    }

    /// <summary>
    /// Gets the member at the specified row index.
    /// </summary>
    public Member? GetMemberAt(int rowIndex)
    {
        if (rowIndex >= 0 && rowIndex < _filteredMembers.Count)
            return _filteredMembers[rowIndex];
        return null;
    }

    /// <summary>
    /// Gets all currently visible (filtered) members.
    /// </summary>
    public IReadOnlyList<Member> GetFilteredMembers() => _filteredMembers;

    /// <summary>
    /// Gets selected members from the grid.
    /// </summary>
    public List<Member> GetSelectedMembers()
    {
        var selected = new List<Member>();
        foreach (DataGridViewRow row in _grid.SelectedRows)
        {
            if (row.Index >= 0 && row.Index < _filteredMembers.Count)
                selected.Add(_filteredMembers[row.Index]);
        }
        return selected;
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(_searchFilter))
        {
            _filteredMembers = _members;
        }
        else
        {
            var filter = _searchFilter.ToLowerInvariant();
            _filteredMembers = _members.Where(m =>
                m.FirstName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                m.LastName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                m.MemberNumber.ToString().Contains(filter) ||
                (m.BusinessRank?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (m.Office?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }

        _grid.RowCount = _filteredMembers.Count;
        _grid.Invalidate();
    }

    private void OnCellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _filteredMembers.Count)
            return;

        var member = _filteredMembers[e.RowIndex];
        var columnName = _grid.Columns[e.ColumnIndex].Name;

        e.Value = columnName switch
        {
            "MemberNumber" => member.MemberNumber,
            "FirstName" => member.FirstName,
            "LastName" => member.LastName,
            "BusinessRank" => member.BusinessRank ?? string.Empty,
            "Office" => member.Office ?? string.Empty,
            "MemberTypeName" => member.MemberType?.DisplayName ?? string.Empty,
            "IsActive" => member.IsActive ? "Yes" : "No",
            _ => string.Empty
        };
    }
}
