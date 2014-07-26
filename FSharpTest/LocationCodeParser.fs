namespace FSharpLib

module LocationCodeParser = 
    open System
    open System.Text.RegularExpressions
    open Interop
    open DataContracts
    open LocationCodeParser.LocationPatterns
    
    // ===== Active patterns ============================
    let (|StartsWith|_|) needle (haystack: string) = 
        if haystack.StartsWith(needle) then Some()
        else None
    
    let (|Contains|_|) needle (haystack: string) = 
        if haystack.Contains(needle) then Some()
        else None
    
    let (|MatchesPattern|_|) pattern code = 
        let matches = Regex.Matches(code, pattern, RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
        if matches.Count > 0 then 
            Some [ for m in matches -> m.Value ]
        else None

    let (|NumericString|_|) string =
        let mutable parseResult = 0.0m
        if Decimal.TryParse(string, ref parseResult) then Some(parseResult)
        else None

    // ===== Cross Feature ==============================
    let parseLeftSide str (filter: LocationFilterMessage) =
        match str with
        | MatchesPattern LocationPatterns.highwayPattern li -> filter.HighwayNumber <- str
        | MatchesPattern LocationPatterns.routePattern li -> filter.RouteNumber <- str
        | _ -> filter.Alias <- str
        filter 

    let parseRightSide str (filter: LocationFilterMessage) =
        let mat = Regex.Match(str, LocationPatterns.roadDirCodePattern)
        filter.CrossFeature <- (mat.Groups.Item(LocationPatterns.PREDECESSOR_GROUP_KEY).Value)
        filter

    let parseXFeature (code: string) (filter: LocationFilterMessage) = 
        code.Split('/')
        |> function
            | [| one; two |] -> parseLeftSide one filter
                                |> parseRightSide two
            | _ -> filter

    // ===== Lat/Long ===================================
    // Sadly, option and Nullable are not the same type.
    let numberOrDefault str =
        let didParse, value = Decimal.TryParse(str)
        if didParse
        then (new Nullable<decimal>(value))
        else (new Nullable<decimal>())

    let parseLatLng (code: string) (filter: LocationFilterMessage) =
        // Remove the leading "=" and split on the comma.
        let pieces = code.Replace("=", "")
                         .Split(',')

        match pieces with
        | [| one; two |] -> filter.LatLong <- (numberOrDefault one, numberOrDefault two)
        | _ -> ()

        filter
    
    // ===== Milepoint ==================================
//    let parseMP code (filter: LocationFilterMessage) = filter
//
//    // ===== Districts ==================================
//    let handleDistricts allDistricts filter = filter
//    
//    /// <summary>
//    /// Top-level parsing logic.  Defers parsing work based on rudimentary structure of the location code.
//    /// </summary>
//    /// <param name="code">The location code to parse</param>
//    let parse code (filter: LocationFilterMessage) = 
//        match code with
//        | Contains "/" () -> parseXFeature code filter
//        | StartsWith "=" () -> parseLatLng code filter
//        | MatchesPattern LocationPatterns.begMPPattern mps -> parseMP mps filter
//        | _ -> new LocationFilterMessage()
//    
//    /// <summary>
//    /// Entry point to the parser.  Ensures a non-null location code is what goes through parsing.
//    /// </summary>
//    /// <param name="code">The location code to parse</param>
//    /// <param name="allDistricts"></param>
//    /// <remarks>
//    /// Because of the nature of F# types, no null values can make it past this point.
//    /// </remarks>
//    let parseLocationCode (code: string, allDistricts: bool) = 
//        let filter = new LocationFilterMessage()
//        match code with
//        | Null code -> filter
//        | _ -> parse (code.Trim()) filter
//               |> handleDistricts allDistricts
