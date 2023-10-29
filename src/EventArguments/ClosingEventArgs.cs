namespace Bau.Controls.DockLayout.EventArguments;

/// <summary>
///		Argumentos del evento de cierre
/// </summary>
public class ClosingEventArgs : EventArgs
{
	public ClosingEventArgs(Models.DockLayoutDocumentModel document)
	{
		Document = document;
	}

	/// <summary>
	///		Documento que lanza el evento de cierre
	/// </summary>
	public Models.DockLayoutDocumentModel Document { get; }

	/// <summary>
	///		Indica si se ha cancelado el cierre
	/// </summary>
	public bool Cancel { get; set; } 
}
