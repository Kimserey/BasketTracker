namespace BasketTracker.Mobile.Core.Storage

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms
open BasketTracker.Mobile.Core.Models

module Stores = 
        
    type StoresApi = {
        List: unit -> Store list
        Add: string -> Store
        Update: int -> string -> unit
        Remove: int-> unit
    }

    let get (storeId: int) = 
        use conn = connect()
        conn.Get<SQLStore>(storeId)
        
    let list () = 
        let sql = """
            SELECT * FROM store 
            WHERE archived <> 1 
            ORDER BY name ASC
        """

        use conn = connect()
        conn.DeferredQuery<SQLStore>(sql, [||]) 
        |> Seq.toList
        |> List.map(fun s -> 
            { Id = s.Id
              Name = s.Name }: Store)
          
    let add name = 
        use conn = connect()
        conn.Insert 
            { Id = 0 
              Name = name
              ImagePath = ""
              Archived = false } |> ignore

        { Id = getLastId()
          Name = name }: Store
  
    let update storeId name = 
        use conn = connect()
        let store = get storeId
        conn.Update { store with Name = name } |> ignore

    let remove storeId =
        use conn = connect()
        let store = get storeId
        conn.Update { store with Archived = true } |> ignore

    let api =
        { List = list
          Add = add
          Update = update
          Remove = remove }