namespace BasketTracker.Mobile.Core.Storage

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms
open BasketTracker.Mobile.Core

[<AutoOpen>]
module Root =

    [<CLIMutable; Table "store">]
    type SQLStore = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "name">]                          Name: string
        [<Column "imagepath">]                     ImagePath: string
        [<Column "archived">]                      Archived: bool
    } 
    
    [<CLIMutable; Table "basket">]
    type SQLBasket = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "date">]                          Date: DateTime
        [<Column "storeid"; Indexed>]              StoreId: int
        [<Column "archived">]                      Archived: bool
    }

    [<CLIMutable; Table "item">]
    type SQLItem = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "amount">]                        Amount: decimal
        [<Column "basketid"; Indexed>]             BasketId: int
        [<Column "archived">]                      Archived: bool
    }

    [<CLIMutable>]
    type SQLBasketQueryResult = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "date">]                          Date: DateTime
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