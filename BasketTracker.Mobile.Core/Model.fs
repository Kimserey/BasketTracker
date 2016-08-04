namespace BasketTracker.Mobile.Core

open System
open Storage

module Model =

    type StoreList(connectionFactory: SQLiteConnectionFactory) =
        
        member self.List() =
            use conn = connectionFactory()
            SQLStore.List conn
            |> List.map (fun s -> new Store(s.Id, s.Name, s.Archived, connectionFactory))

        member self.Add name =
            use conn = connectionFactory()
            SQLStore.Add name conn

    and Store(storeId, name, archived, connectionFactory: SQLiteConnectionFactory) =
        
        let mutable name = name
        let mutable archived = archived
        
        member self.Name
            with get() = name
            and private set value =
                name <- value

        member self.Archived
            with get() = archived
            and private set value =
                archived <- value

        member self.Archive() =
            use conn = connectionFactory()
            SQLStore.Archive storeId conn
            self.Archived <- true

        member self.UpdateName name =
            use conn = connectionFactory()
            SQLStore.Update storeId name conn
            self.Name <- name