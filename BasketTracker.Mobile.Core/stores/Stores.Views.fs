namespace BasketTracker.Mobile.Core.Stores

open BasketTracker.Mobile.Core
open Xamarin.Forms
open System
open System.Globalization
open System.Linq

type StoreListPage(vm: ListPageViewModel, config: ListPageConfiguration, navigator: Navigator) as self =
    inherit ContentPage()

    let listView = 
        new ListView(ItemTemplate = new DataTemplate(fun () -> box (new StoreViewCell(config.Cell, navigator))))
        
    let emptyMsg =
        new Label(
            Text = config.EmptyMessage, 
            XAlign = TextAlignment.Center)

    let add = 
        new ToolbarItem(
            config.Add.Title, 
            config.Add.Icon, 
            fun () -> navigator.Store.NavigateToAdd navigator <| Context self.BindingContext)

    let layout =
        let layout = new StackLayout()
        layout.Children.Add(emptyMsg)
        layout.Children.Add(listView)
        layout

    do
        // Bindings            
        self.SetBinding(ContentPage.TitleProperty, "Title")
        listView.SetBinding(ListView.ItemsSourceProperty, "List")
        emptyMsg.SetBinding(Label.IsVisibleProperty, "List", converter = new IsEmptyConverter())

        // Toolbar items
        self.ToolbarItems.Add(add)

        base.BindingContext <- vm
        base.Content <- layout

    override self.OnAppearing() =
        base.OnAppearing()
        vm.Refresh()
        

and StoreViewCell(config: CellConfiguration, navigator: Navigator) as self =
    inherit ViewCell()

    let name    = new Label(VerticalTextAlignment = TextAlignment.Center)
    let update  = new MenuItem(Text = config.Edit.Title, Icon = FileImageSource.op_Implicit config.Edit.Icon)
    let remove  = new MenuItem(Text = config.Delete.Title, Icon = FileImageSource.op_Implicit config.Delete.Icon)

    let layout = 
        let layout = new StackLayout(Padding = new Thickness(config.Padding))
        layout.Children.Add(name)
        layout

    do
        // Navigation events
        self.Tapped.Add(fun _ -> navigator.Basket.NavigateToBasketList navigator <| Context self.BindingContext)
        update.Clicked.Add(fun _ -> navigator.Store.NavigateToUpdate navigator<| Context  self.BindingContext)
        
        // Bindings
        name.SetBinding(Label.TextProperty, "Name")
        remove.SetBinding(MenuItem.CommandProperty, "RemoveCommand")

        // Context actions
        self.ContextActions.Add(update)
        self.ContextActions.Add(remove)

        self.View <- layout

type AddStorePage(vm, config) as self =
    inherit ModalPage(config)
    
    let entry = new Entry(Placeholder = "Enter the name of the new store here")

    do
        entry.SetBinding(Entry.TextProperty, "Name")
        self.Positive.SetBinding(Button.CommandProperty, "AddCommand")
        self.Positive.SetBinding(Button.CommandParameterProperty, "Name")

        self.BindingContext <- vm
        self.Content <- base.MakeLayout entry
        
type UpdateStorePage(vm, config) as self =
    inherit ModalPage(config)
    
    let entry = new Entry(Placeholder = "Enter a store name here")

    do
        entry.SetBinding(Entry.TextProperty, "Name")
        self.Positive.SetBinding(Button.CommandProperty, "UpdateCommand")
        self.Positive.SetBinding(Button.CommandParameterProperty, "Name")

        self.BindingContext <- vm
        self.Content <- base.MakeLayout entry
