using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InformationSystem.BL.Models;

public abstract record ModelBase : IModel, INotifyPropertyChanged
{
    public Guid Id { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
