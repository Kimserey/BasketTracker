namespace BasketTracker.Mobile.Core

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms

module Storage =

    [<CLIMutable; Table "store">]
    type Store = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "name">]                          Name: string
        [<Column "imagepath">]                     ImagePath: string
        [<Column "archived">]                      Archived: bool
    } with
        static member Add (conn: SQLiteConnection) name = 
            conn.Insert 
                { 
                    Id = 0 
                    Name = name
                    ImagePath = ""
                    Archived = false
                }
            |> ignore
            
        static member Update (storeId: int) name (conn: SQLiteConnection) = 
            conn.RunInTransaction(fun ()->
                let store = conn.Get<Store>(storeId)
                conn.Update { store with Name = name } |> ignore
            )

        static member Get (storeId: int) (conn: SQLiteConnection) = 
            conn.Get<Store>(storeId)

        static member List (conn: SQLiteConnection) = 
            conn.DeferredQuery<Store>("SELECT * FROM store WHERE archived <> 1 ORDER BY name ASC", [||]) |> Seq.toList

    [<CLIMutable; Table "basket">]
    type Basket = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "date">]                          Date: DateTime
        [<Column "storeid"; Indexed>]              StoreId: int
        [<Column "archived">]                      Archived: bool
    }

    [<CLIMutable; Table "item">]
    type Item = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "date">]                          Date: DateTime
        [<Column "amount">]                        Amount: decimal
        [<Column "basketid"; Indexed>]             BasketId: int
        [<Column "archived">]                      Archived: bool
    }

    type SQLiteConnectionFactory = unit -> SQLiteConnection

    let connectionFactory: SQLiteConnectionFactory =
        let appData = 
            DependencyService.Get<IPathsProvider>().ApplicationData
        
        let platform = 
            DependencyService.Get<ISQLitePlatform>()

        fun () ->
            let conn = new SQLiteConnection(platform, Path.Combine(appData, "data.db"))
            conn.CreateTable<Store>() |> ignore
            conn.CreateTable<Basket>() |> ignore
            conn.CreateTable<Item>() |> ignore
            conn