using System;
using System.Windows.Controls;

using AvalonDock.Layout;

namespace Bau.Controls.DockLayout.Models
{
	/// <summary>
	///		Documento o panel asociado a un <see cref="DockLayout"/>
	/// </summary>
	public class DockLayoutDocumentModel : IDisposable
	{
		/// <summary>
		///		Tipo de documento
		/// </summary>
		public enum DocumentType
		{
			/// <summary>Panel</summary>
			Panel,
			/// <summary>Documento / ventana principal</summary>
			Document
		}

		public DockLayoutDocumentModel(string id, string header, DocumentType type, LayoutContent layoutContent, UserControl userControl, object tag = null)
		{
			Id = id;
			Header = header;
			Type = type;
			LayoutContent = layoutContent;
			UserControl = userControl;
			Tag = tag;
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				// Libera la memoria si es necesario
				if (disposing)
				{
					LayoutContent = null;
					UserControl = null;
					Tag = null;
				}
				// Indica que ya se ha liberado la memoria
				Disposed = true;
			}
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		Clave
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header { get; }

		/// <summary>
		///		Tipo de documento
		/// </summary>
		public DocumentType Type { get; }

		/// <summary>
		///		Layout al que se asocia la ventana
		/// </summary>
		public LayoutContent LayoutContent { get; private set; }

		/// <summary>
		///		Control de usuario
		/// </summary>
		public UserControl UserControl { get; private set; }

		/// <summary>
		///		Objeto asociado
		/// </summary>
		public object Tag { get; private set; }

		/// <summary>
		///		Indica si se ha liberado la memoria
		/// </summary>
		public bool Disposed { get; private set; }
	}
}
