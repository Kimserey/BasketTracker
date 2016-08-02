namespace BasketTracker.Mobile.Core

open System
open System.Collections.Generic
open System.Linq
open System.IO
open SQLite
open SQLite.Extensions
open Xamarin.Forms

module Storage =

    [<CLIMutable; Table "store">]
    type Store = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "name">]                          Name: string
    }

    [<CLIMutable; Table "basket">]
    type Basket = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "date">]                          Date: DateTime
        [<Column "storeid"; Indexed>]              StoreId: int
    }

    [<CLIMutable; Table "item">]
    type Item = {
        [<Column "id"; AutoIncrement; PrimaryKey>] Id: int
        [<Column "date">]                          Date: DateTime
        [<Column "amount">]                        Amount: decimal
        [<Column "basketid"; Indexed>]             BasketId: int
    }

    let getConnection() =
        let appData = DependencyService.Get<IPathsProvider>().ApplicationData
        use conn = new SQLiteConnection(Path.Combine(appData, "data.db"))
        conn.CreateTable<Store>() |> ignore
        conn.CreateTable<Basket>() |> ignore
        conn.CreateTable<Item>() |> ignore
        conn
