namespace BasketTracker.Mobile.Core.Items

open BasketTracker.Mobile.Core
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel

type ItemListPage(vm: ListPageViewModel, config: ListPageConfiguration, navigator: Navigator) as self =
    inherit ContentPage()

    let listView = new ListView(ItemTemplate = new DataTemplate(fun () -> box (new ItemViewCell(config.Cell, navigator))))
    
    let emptyMsg =
        new Label(Text = config.EmptyMessage, XAlign = TextAlignment.Center)

    let add = 
        new ToolbarItem(
            config.Add.Title, 
            config.Add.Icon, 
            fun () -> navigator.Item.NavigateToAdd navigator <| Context self.BindingContext)

    let layout = 
        let layout = new StackLayout(Padding = new Thickness(config.Padding))
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

        self.BindingContext <- vm
        self.Content <- layout

    override self.OnAppearing() =
        base.OnAppearing()
        vm.Refresh()


and ItemViewCell(config: CellConfiguration, navigator: Navigator) as self =
    inherit ViewCell()

    let name   = new Label(XAlign = TextAlignment.Start, YAlign = TextAlignment.Center)
    let amount = new Label(XAlign = TextAlignment.End, YAlign = TextAlignment.Center)
    let update  = new MenuItem(Text = config.Edit.Title, Icon = FileImageSource.op_Implicit config.Edit.Icon)
    let remove  = new MenuItem(Text = config.Delete.Title, Icon = FileImageSource.op_Implicit config.Delete.Icon)

    let layout = 
        let layout = new Grid()
        layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(3., GridUnitType.Star)))
        layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        layout.Children.Add(name, 0, 0)
        layout.Children.Add(amount, 1, 0)
        layout

    do
        // Navigation events
        update.Clicked.Add(fun _ -> navigator.Item.NavigateToUpdate navigator <| Context self.BindingContext)

        // Bindings
        name.SetBinding(Label.TextProperty, "Name")
        amount.SetBinding(Label.TextProperty, "Amount", stringFormat = "{0:C2}")
        remove.SetBinding(MenuItem.CommandProperty, "RemoveCommand")

        // Context actions
        self.ContextActions.Add(update)
        self.ContextActions.Add(remove)

        self.View <- layout


type AddItemPage(vm) as self =
    inherit ContentPage()

    let name = new Entry(Placeholder = "Enter the name of the item here")
    let amount = new Entry(Keyboard = Keyboard.Numeric)
    
    let save =
        new ToolbarItem(
            "Save this item", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    let layout =
        let layout = new StackLayout(Padding = new Thickness(10.))
        layout.Children.Add(name)
        layout.Children.Add(amount)
        layout

    do
        // Bindings
        name.SetBinding(Entry.TextProperty, "Name")
        amount.SetBinding(Entry.TextProperty, "Amount")
        self.SetBinding(ContentPage.TitleProperty, "Title") 
        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")
        
        // Toolbar items
        base.ToolbarItems.Add(save)

        self.BindingContext <- vm
        self.Content <- layout


type UpdateItemPage(vm) as self =
    inherit ContentPage()
    
    let name = new Entry(Placeholder = "Enter the name of the item here")
    let amount = new Entry(Keyboard = Keyboard.Numeric)
    
    let save =
        new ToolbarItem(
            "Update this item", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    let layout =
        let layout = new StackLayout(Padding = new Thickness(10.))
        layout.Children.Add(name)
        layout.Children.Add(amount)
        layout

    do
        // Bindings
        name.SetBinding(Entry.TextProperty, "Name")
        amount.SetBinding(Entry.TextProperty, "Amount")
        self.SetBinding(ContentPage.TitleProperty, "Title") 
        save.SetBinding(ToolbarItem.CommandProperty, "UpdateCommand")
        
        // Toolbar items
        base.ToolbarItems.Add(save)

        self.BindingContext <- vm
        self.Content <- layout

