namespace BasketTracker.Mobile.Core.Storage

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms
open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models

type StoresApi = {
    List: unit -> Store list
    Add: string -> Store
    Update: StoreId -> string -> unit
    Remove: StoreId-> unit
}

type BasketsApi = {
    List: StoreId -> Basket list
    Add: StoreId -> DateTime -> Basket
    Update: BasketId -> DateTime -> unit
    Remove: BasketId-> unit
}
    
type ItemsApi = {
    List: BasketId -> Item list
    Add: BasketId -> string -> decimal-> Item
    Update: ItemId -> string -> decimal -> unit
    Remove: ItemId-> unit
} 


[<AutoOpen>]
module Root =
    
    [<CLIMutable; Table "stores">]
    type SQLStore = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "name">]                          Name: string
        [<Column "imagepath">]                     ImagePath: string
        [<Column "archived">]                      Archived: bool
    } 
    
    [<CLIMutable; Table "baskets">]
    type SQLBasket = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "storeid"; Indexed>]              StoreId: int
        [<Column "date">]                          Date: DateTime
        [<Column "archived">]                      Archived: bool
    }

    [<CLIMutable; Table "items">]
    type SQLItem = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "basketid"; Indexed>]             BasketId: int
        [<Column "amount">]                        Amount: decimal
        [<Column "name">]                          Name: string
        [<Column "archived">]                      Archived: bool
    }

    [<CLIMutable>]
    type SQLBasketQueryResult = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "date">]                          Date: DateTime
        [<Column "storeid">]                       StoreId: int
        [<Column "total">]                         Total: decimal
    }

    let connect() =
        let conn = 
            new SQLiteConnection(
                DependencyService.Get<ISQLitePlatform>(), 
                Path.Combine(
                    DependencyService.Get<IPathsProvider>().ApplicationData, 
                    "data.db"
                )
            )
        conn.CreateTable<SQLStore>() |> ignore
        conn.CreateTable<SQLBasket>() |> ignore
        conn.CreateTable<SQLItem>() |> ignore
        conn
    
    let getLastId() =
        use conn = connect()
        conn.ExecuteScalar<int>("SELECT last_insert_rowid()")