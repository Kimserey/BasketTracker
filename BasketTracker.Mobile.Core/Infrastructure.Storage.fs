namespace BasketTracker.Mobile.Core

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Models
open Xamarin.Forms

module Storage =

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
            
    module Stores = 
        
        type StoresApi = {
            List: unit -> Store list
            Add: string -> Store
            Update: int -> string -> Store
            Remove: int-> unit
        }

        let add name = 
            use conn = connect()
            let newStore = 
                { Id = 0 
                  Name = name
                  ImagePath = ""
                  Archived = false }
            conn.Insert newStore |> ignore
            { Id = newStore.Id
              Name = newStore.Name }: Store

        let get (storeId: int) = 
            use conn = connect()
            conn.Get<SQLStore>(storeId)
            
        let update storeId name = 
            use conn = connect()
            let store = get storeId
            conn.Update { store with Name = name } |> ignore
            { Id = store.Id
              Name = store.Name }: Store

        let remove storeId =
            use conn = connect()
            let store = get storeId
            conn.Update { store with Archived = true } |> ignore
        
        let list () = 
            let sql = """
                SELECT * FROM store WHERE archived <> 1 ORDER BY name ASC
            """

            use conn = connect()
            conn.DeferredQuery<SQLStore>(sql, [||]) 
            |> Seq.toList
            |> List.map(fun s -> 
                { Id = s.Id
                  Name = s.Name }: Store)

        let api =
            { List = list
              Add = add
              Update = update
              Remove = remove }


    module Baskets =   
        let list (storeId: int) =
            [ {Id = 1; StoreId = 1; Total = 100.0m; Date = DateTime.Now}
              {Id = 1; StoreId = 1; Total = 100.0m; Date = DateTime.Now.AddHours(5.)} ]

        let list' (storeId: int) =
            let sql = """
                SELECT 
                    b.id id, 
                    b.date date, 
                    SUM(i.amount) total 
                FROM basket b
                LEFT JOIN item i ON b.id = i.basketid
                WHERE 
                    b.archived <> 1
                    AND i.archived <> 1 
                    AND b.storeid = ?
                ORDER BY date DESC
            """

            use conn = connect()
            conn.DeferredQuery<SQLBasketQueryResult>(sql, [| box storeId |]) |> Seq.toList
            |> Seq.toList
            |> List.map (fun b ->
                { Id = b.Id
                  StoreId = storeId
                  Total = b.Total
                  Date = b.Date }:Basket)