namespace BasketTracker.Mobile.Core

type Configuration = {
    Store: ListPageConfiguration
    AddStore: ModalPageConfiguration
    UpdateStore: ModalPageConfiguration

    Basket: ListPageConfiguration
    AddBasket: ModalPageConfiguration
    UpdateBasket: ModalPageConfiguration
    
    Item: ItemListPageConfiguration
    AddItem: ModalPageConfiguration
    UpdateItem: ModalPageConfiguration
} with
    static member Default = { 
        Store = ListPageConfiguration.Default
        AddStore = ModalPageConfiguration.Default
        UpdateStore = ModalPageConfiguration.Default

        Basket = ListPageConfiguration.Default
        AddBasket = ModalPageConfiguration.Default
        UpdateBasket = ModalPageConfiguration.Default
        
        Item = ItemListPageConfiguration.Default
        AddItem = ModalPageConfiguration.Default
        UpdateItem = ModalPageConfiguration.Default
    }

and ItemListPageConfiguration = {
    TitleDateFormat: string
    Padding: float
    Add: ButtonConfig
    Cell: CellConfiguration
    EmptyMessage: string
} with
    static member Default = { 
        TitleDateFormat = "ddd d MMM"
        Padding = 0.
        Add = ButtonConfig.DefaultAdd
        EmptyMessage = "You have not entered any items yet."
        Cell = CellConfiguration.Default 
    }

and ModalPageConfiguration = {
    Padding: float
    Save: ButtonConfig
    Cancel: ButtonConfig
} with
    static member Default = { Padding = 5.; Save = { Title = "Save"; Icon = "" }; Cancel = { Title = "Cancel"; Icon = "" } }

and ListPageConfiguration = {
    Padding: float
    Add: ButtonConfig
    Cell: CellConfiguration
    EmptyMessage: string
} with
    static member Default = { 
        Padding = 0.
        Add = ButtonConfig.DefaultAdd
        EmptyMessage = "You have not entered any items yet."
        Cell = CellConfiguration.Default 
    }

and CellConfiguration = {
    Padding: float
    Edit: ButtonConfig
    Delete: ButtonConfig
} with
    static member Default = { Padding = 5.; Edit = ButtonConfig.DefaultEdit; Delete = ButtonConfig.DefaultDelete }

and ButtonConfig = {
    Title: string
    Icon: string
} with
    static member DefaultAdd = { Title = "Add"; Icon = "add" }
    static member DefaultEdit = { Title = "Edit"; Icon = "pencil" }
    static member DefaultDelete = { Title = "Delete"; Icon = "bin" }