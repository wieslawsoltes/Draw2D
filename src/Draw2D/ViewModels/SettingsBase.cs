using System.Runtime.Serialization;

namespace Draw2D.ViewModels;

[DataContract(IsReference = true)]
public abstract class SettingsBase : ViewModelBase
{
}