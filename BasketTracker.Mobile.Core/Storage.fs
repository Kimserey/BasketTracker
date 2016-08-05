﻿namespace BasketTracker.Mobile.Core

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
        let add name = 
            use conn = connect()
            conn.Insert 
                { 
                    Id = 0 
                    Name = name
                    ImagePath = ""
                    Archived = false
                }
            |> ignore

        let get (storeId: int) = 
            use conn = connect()
            conn.Get<SQLStore>(storeId)
            
        let update storeId name = 
            use conn = connect()
            conn.RunInTransaction (fun () ->
                let store = get storeId
                conn.Update { store with Name = name } |> ignore
            )

        let archive storeId =
            use conn = connect()
            conn.RunInTransaction (fun () ->
                let store = get storeId
                conn.Update { store with Archived = true } |> ignore
            )
        
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


    module Baskets =   
        let list (storeId: int) =
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