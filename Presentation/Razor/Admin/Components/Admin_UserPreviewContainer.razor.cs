namespace Presentation.Razor.Admin.Components;

public partial class Admin_UserPreviewContainer
{
    [Parameter, EditorRequired]
    public User_DTO SelectedUser { get; set; }
}