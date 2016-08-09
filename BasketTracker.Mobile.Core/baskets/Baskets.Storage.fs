namespace BasketTracker.Mobile.Core.Storage

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms
open BasketTracker.Mobile.Core.Models

module Baskets =   
    
    type BasketsApi = {
        List: int -> Basket list
        Add: int -> DateTime -> Basket
        Update: int -> DateTime -> unit
        Remove: int-> unit
    }

    let get (basketId: int) = 
        use conn = connect()
        conn.Get<SQLBasket>(basketId)
    
    let list (storeId: int) =
        let sql = """
            SELECT 
                b.id id, 
                b.date date, 
                (CASE WHEN SUM(i.amount) IS NULL THEN 0 ELSE SUM(i.amount) END) total 
            FROM baskets b
            LEFT JOIN items i ON b.id = i.basketid
            GROUP BY b.id, b.date, b.storeid
            HAVING
                b.archived <> 1 AND b.storeid = ? 
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

    let add (storeId: int) (date: DateTime) =
        use conn = connect()
        conn.Insert 
            { Id = 0
              StoreId = storeId
              Date = date
              Archived = false } |> ignore

        { Id = getLastId()
          StoreId = storeId
          Date = date
          Total = 0.m }: Basket

    let update (basketId: int) date =
        use conn = connect()
        let basket = get basketId
        conn.Update { basket with Date = date } |> ignore

    let remove (basketId: int) =
        use conn = connect()
        let basket = get basketId
        conn.Update { basket with Archived = true } |> ignore

    let api = {
        List = list
        Add = add
        Update = update
        Remove = remove
    }