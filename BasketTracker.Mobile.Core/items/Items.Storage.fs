namespace BasketTracker.Mobile.Core.Items

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage

module Storage =   

    let get (itemId: int) = 
        use conn = connect()
        conn.Get<SQLItem>(itemId)
    
    let list (BasketId basketId) =
        let sql = """
            SELECT *
            FROM items i
            WHERE 
                i.archived <> 1
                && basketid = ?
            ORDER BY i.Name DESC
        """

        use conn = connect()
        conn.DeferredQuery<SQLItem>(sql, [| box basketId |]) |> Seq.toList
        |> Seq.toList
        |> List.map (fun i ->
            { Id = ItemId i.Id
              BasketId = BasketId i.BasketId
              Amount = i.Amount
              Name = i.Name }:Item)

    let add (BasketId basketId) name amount =
        use conn = connect()
        conn.Insert 
            { Id = 0
              BasketId = basketId
              Name = name
              Amount = amount
              Archived = false } |> ignore

        { Id = ItemId <| getLastId()
          BasketId = BasketId basketId
          Name = name
          Amount = amount }: Item

    let update (ItemId itemId) name amount =
        use conn = connect()
        let item = get itemId
        conn.Update { item with Name = name; Amount = amount} |> ignore

    let remove (ItemId itemId) =
        use conn = connect()
        let item = get itemId
        conn.Update { item with Archived = true } |> ignore

    let api: ItemsApi = {
        List = list
        Add = add
        Update = update
        Remove = remove
    }