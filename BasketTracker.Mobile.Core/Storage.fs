namespace BasketTracker.Mobile.Core

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms

module Storage =

    [<CLIMutable; Table "store">]
    type SQLStore = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "name">]                          Name: string
        [<Column "imagepath">]                     ImagePath: string
        [<Column "archived">]                      Archived: bool
    } with
        static member Add name (conn: SQLiteConnection) = 
            conn.Insert 
                { 
                    Id = 0 
                    Name = name
                    ImagePath = ""
                    Archived = false
                }
            |> ignore

        static member Get (storeId: int) (conn: SQLiteConnection) = 
            conn.Get<SQLStore>(storeId)
            
        static member Update storeId name (conn: SQLiteConnection) = 
            conn.RunInTransaction (fun () ->
                let store = SQLStore.Get storeId conn
                conn.Update { store with Name = name } |> ignore
            )

        static member Archive storeId (conn: SQLiteConnection) =
            conn.RunInTransaction (fun () ->
                let store = SQLStore.Get storeId conn
                conn.Update { store with Archived = true } |> ignore
            )
        static member List (conn: SQLiteConnection) = 
            conn.DeferredQuery<SQLStore>("SELECT * FROM store WHERE archived <> 1 ORDER BY name ASC", [||]) |> Seq.toList

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
            conn.CreateTable<SQLStore>() |> ignore
            conn.CreateTable<SQLBasket>() |> ignore
            conn.CreateTable<SQLItem>() |> ignore
            conn