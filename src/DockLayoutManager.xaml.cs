using System.Windows;
using System.Windows.Controls;

using AvalonDock.Layout;
using Bau.Controls.DockLayout.Models;

namespace Bau.Controls.DockLayout;

/// <summary>
///		Manager / wrapper sobre AvalonDock
/// </summary>
public partial class DockLayoutManager : UserControl
{
	/// <summary>
	///		Posición en la que se puede colocar un panel
	/// </summary>
	public enum DockPosition
	{
		/// <summary>Izquierda</summary>
		Left,
		/// <summary>Superior</summary>
		Top,
		/// <summary>Derecha</summary>
		Right,
		/// <summary>Abajo</summary>
		Bottom
	}
	/// <summary>
	///		Temas de las ventanas
	/// </summary>
	public enum DockTheme
	{
		Aero,
		Metro,
		VS2010Theme,
		ExpressionDark,
		ExpressionLight,
		VS2013LightTheme,
		VS2013BlueTheme,
		VS2013DarkTheme
	}
	// Eventos públicos
	public event EventHandler<EventArguments.OpenFileRequiredArgs> OpenFileRequired;
	public event EventHandler<EventArguments.ClosingEventArgs> Closing;
	public event EventHandler<EventArguments.ClosedEventArgs> Closed;
	public event EventHandler ActiveDocumentChanged;
	// Variables privadas
	private DockLayoutDocumentModel? _activeDocument;

	public DockLayoutManager()
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna el contexto de los documentos
		dckManager.DocumentsSource = Documents;
		// Inicializa los manejadores de eventos
		dckManager.ActiveContentChanged += (sender, evntArgs) =>
													{
														if (dckManager.ActiveContent != null && dckManager.Layout != null && dckManager.Layout.ActiveContent != null)
															ActiveDocument = GetDocument(dckManager.Layout.ActiveContent.ContentId);
														else
															ActiveDocument = null;
													};
	}

	/// <summary>
	///		Añade / selecciona un panel
	/// </summary>
	public void AddPane(string id, string header, UserControl control, object? tag = null, DockPosition position = DockPosition.Bottom)
	{
		DockLayoutDocumentModel previous = GetDocument(id);

			if (previous is not null)
			{
				if (previous.LayoutContent is not null)
					previous.LayoutContent.IsActive = true;
				ActiveDocument = previous;
			}
			else
			{
				LayoutAnchorGroup layoutGroup = GetGroupPane(dckManager.Layout, position);
				LayoutAnchorable layoutAnchorable = new LayoutAnchorable { Title = header, ToolTip = header };

					// Añade el contenido
					layoutAnchorable.Content = control;
					layoutAnchorable.ContentId = id;
					// Asigna los parámetros del panel
					layoutAnchorable.FloatingHeight = 200;
					layoutAnchorable.FloatingWidth = 200;
					layoutAnchorable.AutoHideWidth = 400;
					// Añade el contenido al grupo
					layoutGroup.Children.Add(layoutAnchorable);
					layoutAnchorable.IsActive = true;
					layoutAnchorable.IsVisible = true;
					// Añade el panel a la lista de documentos del controlador
					AddDocument(id, header, DockLayoutDocumentModel.DocumentType.Panel, layoutAnchorable, control, tag);
			}
	}

	/// <summary>
	///		Abre / cierra un panel lateral
	/// </summary>
	public void OpenGroup(DockPosition position)
	{
		LayoutAnchorGroup layoutGroup = GetGroupPane(dckManager.Layout, position);

			// Abre el panel
			if (layoutGroup.Children.Count > 0)
			{
				// Cambia el ancho / alto del grupo
				switch (position)
				{
					case DockPosition.Left:
					case DockPosition.Right:
							layoutGroup.Children[0].FloatingWidth = dckManager.ActualWidth / 4;
							layoutGroup.Children[0].AutoHideWidth = dckManager.ActualWidth / 4;
							layoutGroup.Children[0].AutoHideMinWidth = 200;
						break;
					default:
							layoutGroup.Children[0].FloatingHeight = dckManager.ActualHeight / 4;
							layoutGroup.Children[0].AutoHideHeight = dckManager.ActualHeight / 6;
							layoutGroup.Children[0].AutoHideMinHeight = 200;
						break;
				}
				// Cambia el autohide para que aparezca
				//? Después de ejecutar esta instrucción, parece que se cambia el grupo, por tanto ya no podemos utilizar layoutGroup.Children[0]
				//? que provocaría una excepción
				layoutGroup.Children[0].ToggleAutoHide();
			}
	}

	/// <summary>
	///		Añade / selecciona un documento
	/// </summary>
	public void AddDocument(string id, string header, UserControl control, object tag = null)
	{
		DockLayoutDocumentModel previous = GetDocument(id);

			if (previous != null)
			{
				previous.LayoutContent.IsActive = true;
				ActiveDocument = previous;
			}
			else
			{
				LayoutDocumentPane documentPane = dckManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
				LayoutDocument layoutDocument = new LayoutDocument { Title = header, ToolTip = header };

					// Crea un documento y le asigna el control de contenido
					if (documentPane is not null)
					{ 
						// Asigna el control
						layoutDocument.Content = control;
						layoutDocument.ContentId = id;
						// Añade el nuevo LayoutDocument al array existente
						documentPane.Children.Add(layoutDocument);
						// Activa el documento
						layoutDocument.IsActive = true;
						layoutDocument.IsSelected = true;
						// Cambia el ancho y alto flotante
						layoutDocument.FloatingWidth = dckManager.ActualWidth / 2;
						layoutDocument.FloatingHeight = dckManager.ActualHeight;
						// Cambia el foco al control
						control.Focus();
						// Añade el documento al controlador
						AddDocument(id, header, DockLayoutDocumentModel.DocumentType.Document, layoutDocument, control, tag);
					}
			}
	}

	/// <summary>
	///		Obtiene el <see cref="DockLayoutDocumentModel"/> a partir de un ID de ventana
	/// </summary>
	private DockLayoutDocumentModel? GetDocument(string id)
	{
		if (Documents.TryGetValue(id, out DockLayoutDocumentModel? document))
			return document;
		else
			return null;
	}

	/// <summary>
	///		Modifica el tabId de un documento activo
	/// </summary>
	public void UpdateTabId(string oldTabId, string newTabId, string newHeader)
	{
		if (Documents.TryGetValue(oldTabId, out DockLayoutDocumentModel? document) && !Documents.ContainsKey(newTabId))
		{
			// Cambia el título
			document.LayoutContent.Title = newHeader;
			// Añade el nuevo Id al diccionario y le cambia el Id interno
			Documents.Add(newTabId, document);
			document.Id = newTabId;
			// Elimina el documento con el Id antiguo
			Documents.Remove(oldTabId);
		}
	}

	/// <summary>
	///		Añade un documento al control o lo selecciona
	/// </summary>
	private void AddDocument(string id, string header, DockLayoutDocumentModel.DocumentType type, 
							 LayoutContent layoutContent, UserControl userControl, object tag = null)
	{ 
		DockLayoutDocumentModel document = new DockLayoutDocumentModel(id, header, type, layoutContent, userControl, tag);

			// Añade el documento al diccionario
			Documents.Add(document.Id, document);
			// Asigna los manejadores de evento a la vista
			layoutContent.Closing += (sender, args) => args.Cancel = !TreatEventCloseForm(document);
			layoutContent.IsActiveChanged += (sender, args) => RaiseEventChangeDocument(document);
			// Activa el documento
			document.LayoutContent.IsActive = true;
			ActiveDocument = document;
	}

	/// <summary>
	///		Obtiene el grupo de ventanas del panel de la posición especificada
	/// </summary>
	private LayoutAnchorGroup GetGroupPane(LayoutRoot layoutRoot, DockPosition position)
	{
		LayoutAnchorSide layoutSide = GetAnchorSide(layoutRoot, position);

			// Crea el panel si no existía
			if (layoutSide == null)
				layoutSide = new LayoutAnchorSide();
			// Añade un grupo si no existía
			if (layoutSide.Children == null || layoutSide.Children.Count == 0)
				layoutSide.Children.Add(new LayoutAnchorGroup());
			// Devuelve el primer grupo
			return layoutSide.Children[0];
	}

	/// <summary>
	///		Obtiene uno de los elementos laterales del gestor de ventanas
	/// </summary>
	private LayoutAnchorSide GetAnchorSide(LayoutRoot layoutRoot, DockPosition position)
	{
		return position switch
					{
						DockPosition.Right => layoutRoot.RightSide,
						DockPosition.Left => layoutRoot.LeftSide,
						DockPosition.Top => layoutRoot.TopSide,
						_ => layoutRoot.BottomSide
					};
	}

	/// <summary>
	///		Trata el evento de cierre de un documento
	/// </summary>
	private bool TreatEventCloseForm(DockLayoutDocumentModel document)
	{
		bool canClose = true;

			// Si es un documento, antes de cerrar se pregunta a la ventana principal
			if (document.Type == DockLayoutDocumentModel.DocumentType.Document)
			{
				EventArguments.ClosingEventArgs args = new EventArguments.ClosingEventArgs(document);

					// Llama al manejador de eventos
					Closing?.Invoke(this, args);
					// Si no se ha cancelado, se puede cerrar
					canClose = !args.Cancel;
			}
			// Si se debe cerrar, se quita del diccionario
			if (canClose)
				RemoveTab(document);
			// Devuelve el valor que indica si se puede cerrar el documento
			return canClose;
	}

	/// <summary>
	///		Cierra una ventana
	/// </summary>
	public void CloseTab(string tabId)
	{
		if (Documents.TryGetValue(tabId, out DockLayoutDocumentModel document))
			RemoveTab(document);
	}

	/// <summary>
	///		Elimina una ficha
	/// </summary>
	private void RemoveTab(DockLayoutDocumentModel document)
	{
		// Elimina los manejadores de eventos asociados al documento
		EventManager.EventReflectionService.RemoveAllEventHandlers(document);
		EventManager.EventReflectionService.RemoveAllEventHandlers(document.LayoutContent);
		EventManager.EventReflectionService.RemoveAllEventHandlers(document.UserControl);
		// Lanza el evento de cierre (antes de cerrar el layout, para que se liberen los recursos)
		RaiseEventClosedDocument(document);
		// Cierra el documento
		document.LayoutContent.Close();
		// Quita el elemento del diccionario
		if (Documents.ContainsKey(document.Id))
			Documents.Remove(document.Id);
		// Libera la memoria
		document.Dispose();
	}

	/// <summary>
	///		Lanza el evento de documento cerrado (antes de cerrar el layout)
	/// </summary>
	private void RaiseEventClosedDocument(DockLayoutDocumentModel document)
	{
		Closed?.Invoke(this, new EventArguments.ClosedEventArgs(document));
	}

	/// <summary>
	///		Cierra todas las ventanas
	/// </summary>
	public void CloseAllDocuments()
	{
		List<DockLayoutDocumentModel> documents = new();

			// Nota: Al cerrar un formulario se modifica la colección Documents, por tanto no se puede hacer un recorrido sobre Documents
			//			 porque da un error de colección modificada. Tampoco se puede hacer un recorrido for (int...) sobre Documents porque
			//			 es un diccionario y no tiene indizador. Por tanto, tenemos que copiar los elementos del diccionario sobre una lista
			//			 y después recorrer los elementos de esta lista copiada desde el final hasta el principio.
			// Añade el diccionario de documentos a la lista
			foreach (KeyValuePair<string, DockLayoutDocumentModel> document in Documents)
				documents.Add(document.Value);
			// Recorre la lista cerrando todos los documentos abiertos
			for (int index = documents.Count - 1; index >= 0; index--)
				if (documents[index] != null && documents[index].Type == DockLayoutDocumentModel.DocumentType.Document && 
						documents[index].LayoutContent != null)
					try
					{
						documents[index]?.LayoutContent?.Close();
					}
					catch { }
	}

	/// <summary>
	///		Obtiene los tag de las vistas abiertas
	/// </summary>
	public List<object> GetOpenedViews()
	{
		List<object> tags = new List<object>();

			// Obtiene los objetos asociados a los documentos abiertos
			foreach (KeyValuePair<string, DockLayoutDocumentModel> document in Documents)
				if (document.Value.Tag != null)
					tags.Add(document.Value.Tag);
			// Devuelve la lista
			return tags;
	}

	/// <summary>
	///		Cambia el tema del layout
	/// </summary>
	public void SetTheme(DockTheme theme)
	{
		switch (theme)
		{
			case DockTheme.Aero:
					dckManager.Theme = new AvalonDock.Themes.AeroTheme();
				break;
			case DockTheme.ExpressionDark:
					dckManager.Theme = new AvalonDock.Themes.ExpressionDarkTheme();
				break;
			case DockTheme.ExpressionLight:
					dckManager.Theme = new AvalonDock.Themes.ExpressionLightTheme();
				break;
			case DockTheme.Metro:
					dckManager.Theme = new AvalonDock.Themes.MetroTheme();
				break;
			case DockTheme.VS2013LightTheme:
					dckManager.Theme = new AvalonDock.Themes.Vs2013LightTheme();
				break;
			case DockTheme.VS2013BlueTheme:
					dckManager.Theme = new AvalonDock.Themes.Vs2013BlueTheme();
				break;
			case DockTheme.VS2013DarkTheme:
					dckManager.Theme = new AvalonDock.Themes.Vs2013DarkTheme();
				break;
			default:
					dckManager.Theme = new AvalonDock.Themes.VS2010Theme();
				break;
		}
	}

	/// <summary>
	///		Lanza el evento de cambio de documento
	/// </summary>
	private void RaiseEventChangeDocument(DockLayoutDocumentModel? document)
	{
		if (document?.LayoutContent?.IsActive ?? false)
			ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Documentos
	/// </summary>
	public Dictionary<string, DockLayoutDocumentModel> Documents { get; } = new();

	/// <summary>
	///		Documento activo
	/// </summary>
	public DockLayoutDocumentModel? ActiveDocument
	{
		get { return _activeDocument; }
		private set
		{
			_activeDocument = value;
			RaiseEventChangeDocument(value);
		}
	}

	private void dckManager_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
			try
			{
				// Lanza los eventos para abrir archivos
				foreach (string file in (string []) e.Data.GetData(DataFormats.FileDrop))
					if (!string.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file))
						OpenFileRequired?.Invoke(this, new EventArguments.OpenFileRequiredArgs(file));
				// Indica que se ha tratado el evento
				e.Handled = true;
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Error when drop files. {exception.Message}");
			}
	}
}