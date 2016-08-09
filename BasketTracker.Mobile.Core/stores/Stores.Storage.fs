namespace BasketTracker.Mobile.Core.Stores

open System
open System.IO
open SQLite.Net
open SQLite.Net.Interop
open SQLite.Net.Attributes
open Xamarin.Forms
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage

module Storage = 

    let get (storeId: int) = 
        use conn = connect()
        conn.Get<SQLStore>(storeId)
        
    let list () = 
        let sql = """
            SELECT * FROM stores
            WHERE archived <> 1 
            ORDER BY name ASC
        """

        use conn = connect()
        conn.DeferredQuery<SQLStore>(sql, [||]) 
        |> Seq.toList
        |> List.map(fun s -> 
            { Id = StoreId s.Id
              Name = s.Name }: Store)
          
    let add name = 
        use conn = connect()
        conn.Insert 
            { Id = 0 
              Name = name
              ImagePath = ""
              Archived = false } |> ignore

        { Id = StoreId <| getLastId()
          Name = name }: Store
  
    let update (StoreId storeId) name = 
        use conn = connect()
        let store = get storeId
        conn.Update { store with Name = name } |> ignore

    let remove (StoreId storeId) =
        use conn = connect()
        let store = get storeId
        conn.Update { store with Archived = true } |> ignore

    let api: StoresApi =
        { List = list
          Add = add
          Update = update
          Remove = remove }