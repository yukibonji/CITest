
module Elm = 
  type Name = string 

  type Associativity = Left | Right | Nothing 

  type Binop =  Associativity * int * string

  type ModuleName = string list 

  type Alias = string

  type Comment = Comment of string 

  type ExportSet 
    = ExportAll
    | ExportSubset of ExportSet list
    | ExportFunction of Name 
    | ExportType of string * ExportSet option 


  type Ty
    = TyCon of Name list  * Ty list
    | TyVar of Name 
    | TyRecordCon of Ty * (Name * Ty ) list 
    | TyRecord of (Name * Ty) list 
    | TyTuple of Ty list 
    | TyAp of Ty * Ty 


  type Literal 
    = Character of char
    | String of string
    | Integer of int
    | Float of float

  type Expr
    = Lit of Literal 
    | Variable of Name list 
    | List of Expr list
    | Tuple of Expr list 
    | Access of Expr * Name list
    | AccessFunction of fieldName : Name
    | Record of fields : ( Name * Expr ) list 
    | RecordUpdate of recordName: Name * fields : ( Name * Expr ) list 
    | If of cond : Expr * t: Expr * f : Expr
    | Let of  exprs : (Expr * Expr) list * body : Expr
    | Case of test : Expr * cases: (Expr * Expr) list
    | Lambda of args: Expr list * body: Expr 
    | Application of lhs : Expr * rhs : Expr 
    | BinOp of lhs : Expr * op : Expr * rhs: Expr


  type Decl 
      = TypeAliasDecl of Ty * Ty
      | TypeDecl of Ty * Ty list
      | PortTypeDecl of Name *  Ty
      | PortDecl of Name *  Name list * Expr 
      | FunctionSigDecl of Name * Ty
      | FunctionDecl of Name * Expr list * Expr
      | InfixDecl of Associativity *  int * Name
      | ImportDecl of ModuleName *  Alias option * ExportSet option

  type Module 
    = Module of ModuleName * ExportSet * Comment option * (Comment option * Decl) list 
    | PortModule of  ModuleName *  ExportSet * Comment option * (Comment option * Decl) list 
    | EffectModule of ModuleName * ( Name *  Name) list * ExportSet * Comment option * (Comment option * Decl) list 
    
module Decoder = 
  open Elm

  type DecoderEnv = 
    Map<Ty , Expr>
  

  let field = 
    Access(Variable["Json";"Decode"],["field"])
  let mkField label decoderExpr = 
    Application
          ( Application
              (field 
              , Lit <| String label
              )
          ,  decoderExpr
          )
  let map n = 
    Access(Variable["Json";"Decode"],[sprintf "map%i" n])
  let mkMap fieldCount constructor decoders = 
    List.fold
      (fun lhs rhs -> Application(lhs,rhs)) 
        (Application(map fieldCount, constructor)) 
          decoders

  let rec ofTypeDecl decl env = 
    match decl with
    | TypeDecl( TyCon(ns,tvars), tyCons) ->
        ofDU (ns, tvars) tyCons env

    | TypeAliasDecl( TyCon(ns , tvars), TyRecord fields ) ->
        ofRecord (ns, tvars) fields env 

    | _ -> 
      (None, env)

  and ofDU (ns , tvars) tyCons env = 
    (None, env)

  and ofRecord (ns, tvars) fields env : Expr option * Map<Ty,Expr> = 
    let n = List.length fields 
    if n > 8 then
      failwith <| 
        sprintf 
          "Cannot generate decoder for type alias %A - too many fields" 
          (Variable ns)

    else       
      let (env', fs) = 
        List.fold
            (fun (env,accu) field ->
              let (fld,env') = ofField field env 
              (env', fld::accu)
            ) (env, []) fields 

      ( Some <| (mkMap n (Variable ns) <| List.rev fs )
      , env'
      )


  and ofField (label, ty) env = 
    match Map.tryFind ty env with
    | Some decoderExpr -> 
      ( mkField label decoderExpr 
      , env
      )
    | _ ->
      match ofTypeDecl ty env with
      | Some decoderExpr,env' ->
          ( mkField label decoderExpr 
          , Map.add ty decoderExpr env'
          )
      | _ ->
        failwith <| sprintf "Could not generate decoder for %A" ty

        

  



