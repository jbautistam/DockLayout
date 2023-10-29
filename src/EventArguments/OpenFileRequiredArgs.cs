namespace Bau.Controls.DockLayout.EventArguments;

/// <summary>
///		Argumentos del evento <see cref="DockLayoutManager.OpenFileRequired"/>
/// </summary>
public class OpenFileRequiredArgs : EventArgs
{
	public OpenFileRequiredArgs(string fileName)
	{
		FileName = fileName;
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; }
}
