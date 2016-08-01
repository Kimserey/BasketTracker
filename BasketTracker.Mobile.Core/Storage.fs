namespace BasketTracker.Mobile.Core

open System
open System.Collections.Generic
open System.Linq
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
