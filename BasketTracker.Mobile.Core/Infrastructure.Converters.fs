namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.ComponentModel
open System.Collections.ObjectModel
open Xamarin.Forms

type IsEmptyConverter() =
    interface IValueConverter with 
        member self.Convert(value, targetType, param, culture) =
            let list = unbox<seq<obj>> value 
            box <| Seq.isEmpty list
                        
        member self.ConvertBack(value, targetType, param, culture) =
            failwith "Converting back from bool to list is not supported."
