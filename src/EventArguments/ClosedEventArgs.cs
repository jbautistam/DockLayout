namespace Bau.Controls.DockLayout.EventArguments;

/// <summary>
///		Argumentos del evento que se lanza una vez cerrado el documento
/// </summary>
public class ClosedEventArgs : EventArgs
{
	public ClosedEventArgs(Models.DockLayoutDocumentModel document)
	{
		Document = document;
	}

	/// <summary>
	///		Documento que se ha cerrado
	/// </summary>
	public Models.DockLayoutDocumentModel Document { get; }
}
