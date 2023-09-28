
using CommunityToolkit.Mvvm.Input;

namespace Draw2D.ViewModels.Tools;

public interface ISelection : IDirty
{
    IRelayCommand<IToolContext> CutCommand { get; }
    IRelayCommand<IToolContext> CopyCommand { get; }
    IRelayCommand<IToolContext> PasteCommand { get; }
    IRelayCommand<IToolContext> DeleteCommand { get; }
    IRelayCommand<IToolContext> DuplicateCommand { get; }
    IRelayCommand<IToolContext> CreateGroupCommand { get; }
    IRelayCommand<IToolContext> CreateReferenceCommand { get; }
    IRelayCommand<IToolContext> CreatePathCommand { get; }
    IRelayCommand<IToolContext> CreateStrokePathCommand { get; }
    IRelayCommand<IToolContext> CreateFillPathCommand { get; }
    IRelayCommand<IToolContext> StackHorizontallyCommand { get; }
    IRelayCommand<IToolContext> StackVerticallyCommand { get; }
    IRelayCommand<IToolContext> DistributeHorizontallyCommand { get; }
    IRelayCommand<IToolContext> DistributeVerticallyCommand { get; }
    IRelayCommand<IToolContext> AlignLeftCommand { get; }
    IRelayCommand<IToolContext> AlignCenteredCommand { get; }
    IRelayCommand<IToolContext> AlignRightCommand { get; }
    IRelayCommand<IToolContext> AlignTopCommand { get; }
    IRelayCommand<IToolContext> AlignCenterCommand { get; }
    IRelayCommand<IToolContext> AlignBottomCommand { get; }
    IRelayCommand<IToolContext> ArangeBringToFrontCommand { get; }
    IRelayCommand<IToolContext> ArangeBringForwardCommand { get; }
    IRelayCommand<IToolContext> ArangeSendBackwardCommand { get; }
    IRelayCommand<IToolContext> ArangeSendToBackCommand { get; }
    IRelayCommand<IToolContext> BreakCommand { get; }
    IRelayCommand<IToolContext> ConnectCommand { get; }
    IRelayCommand<IToolContext> DisconnectCommand { get; }
    IRelayCommand<IToolContext> SelectAllCommand { get; }
    IRelayCommand<IToolContext> DeselectAllCommand { get; }
    void Cut(IToolContext context);
    void Copy(IToolContext context);
    void Paste(IToolContext context);
    void Delete(IToolContext context);
    void Duplicate(IToolContext context);
    void CreateGroup(IToolContext context);
    void CreateReference(IToolContext context);
    void CreatePath(IToolContext context);
    void CreateStrokePath(IToolContext context);
    void CreateFillPath(IToolContext context);
    void StackHorizontally(IToolContext context);
    void StackVertically(IToolContext context);
    void DistributeHorizontally(IToolContext context);
    void DistributeVertically(IToolContext context);
    void AlignLeft(IToolContext context);
    void AlignCentered(IToolContext context);
    void AlignRight(IToolContext context);
    void AlignTop(IToolContext context);
    void AlignCenter(IToolContext context);
    void AlignBottom(IToolContext context);
    void ArangeBringToFront(IToolContext context);
    void ArangeBringForward(IToolContext context);
    void ArangeSendBackward(IToolContext context);
    void ArangeSendToBack(IToolContext context);
    void Break(IToolContext context);
    void Connect(IToolContext context);
    void Disconnect(IToolContext context);
    void SelectAll(IToolContext context);
    void DeselectAll(IToolContext context);
}
