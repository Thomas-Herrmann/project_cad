﻿namespace ProjectCadLanguage

open Microsoft.FSharp.Collections
open System
open DecimalMath

type Constant = True | False | Not | Less | Greater | LessEq | GreaterEq | Eq | NotEq | Land | Lor // Boolean operators
              | Plus | Minus | UnaryMinus | Multiplication | Division | Power | Root             // Arithmetic operators
              | Subscript | Column // Matrix operators

type Exp = NumExp of decimal
         | ConstExp of Constant
         | VarExp of string
         | AppExp of Exp * Exp
         | FunExp of string * Exp
         | MatrixExp of Exp[,] * int * int
         | GuardExp of (Option<Exp> * Exp) List
         | HoleExp of int

type Value = NumValue of decimal
           | ConstantValue of Constant * Option<Value -> Value>
           | FunValue of string * Exp
           | MatrixValue of Value[,] * int * int
           | BooleanValue of bool

type Env = Map<string, Value>


module Language = // TODO: arithmetic operators on matrices

    let rec valueToString = function
        | NumValue n -> n.ToString()
        | ConstantValue (c, _) -> "TODO"
        | FunValue (x, _) -> "fun x"
        | BooleanValue b -> if b then "true" else "false"
        | MatrixValue (vs, r, c) -> 
            seq { for i in 0..r-1 ->
                    let row = seq { for j in 0..c-1 -> (valueToString (vs.[i, j])) + " " } 
                    in Seq.append row ["; "] |> String.Concat
                } |> String.Concat

    let applyConstant c v' v2 = 
        match c, v', v2 with
        | Not, None, BooleanValue b -> BooleanValue (not b)
        | UnaryMinus, None, NumValue n -> NumValue (-n)

        | Less, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) < 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "< cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | Greater, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) > 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "> cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | LessEq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) <= 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "<= cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | GreaterEq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) >= 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf ">= cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | Eq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) = 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "== cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | NotEq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) <> 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "<> cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | Land, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | BooleanValue b1, BooleanValue b2 -> if b1 && b2 then BooleanValue true else BooleanValue false
                | _ -> failwithf "^ cannot be applied to a non-boolean value."
            in ConstantValue (c, Some (comp v))

        | Lor, None, v ->
                   let comp v1 v2 = 
                       match v1, v2 with 
                       | BooleanValue b1, BooleanValue b2 -> if b1 || b2 then BooleanValue true else BooleanValue false
                       | _ -> failwithf "v cannot be applied to a non-boolean value."
                   in ConstantValue (c, Some (comp v))

        | Plus, None, v ->
                let comp v1 v2 =
                    match v1, v2 with
                    | NumValue n1, NumValue n2 -> NumValue (n1 + n2)
                    | NumValue n, MatrixValue (m, r, c) | MatrixValue (m, r, c), NumValue n ->
                        let f = function 
                            | NumValue n' -> NumValue (n + n') 
                            | _ -> failwithf "+ cannot be applied to a non-decimal matrix."
                        in MatrixValue (Array2D.map f m, r, c)
                    | MatrixValue (m1, r1, c1), MatrixValue (m2, r2, c2) ->
                        if r1 = r2 && c1 = c2
                        then let f = function
                                | NumValue n1, NumValue n2 -> NumValue (n1 + n2)
                                | _ -> failwithf "+ cannot be applied to a non-decimal matrix."
                             in MatrixValue (array2D [| for i in 0..r1-1 -> [| for j in 0..c1-1 -> f(m1.[i, j], m2.[i, j]) |] |] , r1, c1)
                        else failwithf "+ cannot be applied to matrices with non-matching dimensions."
                    | _ -> failwithf "+ cannot be applied to a non-decimal value."
                in ConstantValue (c, Some (comp v))

        | Minus, None, v ->
            let comp v1 v2 =
                match v1, v2 with
                | NumValue n1, NumValue n2 -> NumValue (n1 - n2)
                | NumValue n, MatrixValue (m, r, c) ->
                    let f = function
                        | NumValue n' -> NumValue (n - n')
                        | _ -> failwithf "- cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | MatrixValue (m, r, c), NumValue n ->
                    let f = function
                        | NumValue n' -> NumValue (n' - n)
                        | _ -> failwithf "- cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | MatrixValue (m1, r1, c1), MatrixValue (m2, r2, c2) ->
                    if r1 = r2 && c1 = c2
                    then let f = function
                            | NumValue n1, NumValue n2 -> NumValue (n1 - n2)
                            | _ -> failwithf "- cannot be applied to a non-decimal matrix."
                         in MatrixValue (array2D [| for i in 0..r1-1 -> [| for j in 0..c1-1 -> f(m1.[i, j], m2.[i, j]) |] |] , r1, c1)
                    else failwithf "- cannot be applied to matrices with non-matching dimensions."
                | _ -> failwithf "- cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | Multiplication, None, v ->
            let comp v1 v2 =
                match v1, v2 with
                | NumValue n1, NumValue n2 -> NumValue (n1 * n2)
                | NumValue n, MatrixValue (m, r, c) | MatrixValue (m, r, c), NumValue n ->
                    let f = function 
                        | NumValue n' -> NumValue (n * n') 
                        | _ -> failwithf "* cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | MatrixValue (m1, r1, c1), MatrixValue (m2, r2, c2) ->
                    if c1 = r2
                    then let f = function
                            | NumValue n1, NumValue n2 -> n1 * n2
                            | _ -> failwithf "* cannot be applied to non-decimal matrices."
                         in MatrixValue (array2D [| for i in 0..r1-1 -> [| for j in 0..c2-1 -> NumValue (Array.fold (+) 0M [| for k in 0..c1-1 -> f(m1.[i, k], m2.[k, j]) |]) |] |], r1, c2)
                    else failwithf "* cannot be applied to matrices with non-matching dimensions."
                | _ -> failwithf "* cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))
        
        | Division, None, v ->
            let comp v1 v2 =
                match v1, v2 with
                | NumValue n1, NumValue n2 -> NumValue (n1 / n2)
                | NumValue n, MatrixValue (m, r, c) ->
                    let f = function
                        | NumValue n' -> NumValue (n / n')
                        | _ -> failwithf "/ cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | MatrixValue (m, r, c), NumValue n ->
                    let f = function
                        | NumValue n' -> NumValue (n' / n)
                        | _ -> failwithf "/ cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | _ -> failwithf "/ cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | Power, None, v ->
            let comp v1 v2 =
                match v1, v2 with
                | NumValue n1, NumValue n2 -> NumValue (DecimalMath.precisionPower n1 n2 0.0000001)
                | NumValue n, MatrixValue (m, r, c) ->
                    let f = function
                        | NumValue n' -> NumValue (DecimalMath.precisionPower n n' 0.0000001)
                        | _ -> failwithf "** cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | MatrixValue (m, r, c), NumValue n ->
                    let f = function
                        | NumValue n' -> NumValue (DecimalMath.precisionPower n' n 0.0000001)
                        | _ -> failwithf "** cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | _ -> failwithf "** cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | Root, None, v ->
            let comp v1 v2 =
                match v1, v2 with
                | NumValue n1, NumValue n2 -> NumValue (DecimalMath.precisionRoot n1 n2 0.0000001)
                | NumValue n, MatrixValue (m, r, c) ->
                    let f = function
                        | NumValue n' -> NumValue (DecimalMath.precisionRoot n n' 0.0000001)
                        | _ -> failwithf "Root cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | MatrixValue (m, r, c), NumValue n ->
                    let f = function
                        | NumValue n' -> NumValue (DecimalMath.precisionRoot n' n 0.0000001)
                        | _ -> failwithf "Root cannot be applied to a non-decimal matrix."
                    in MatrixValue (Array2D.map f m, r, c)
                | _ -> failwithf "Root cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))

        | Subscript, None, v ->
            let comp v1 v2 =
                match v1, v2 with
                | MatrixValue (m, r, c), NumValue n -> 
                    if Decimal.Round(n) = n 
                    then let row = Decimal.ToInt32(n) 
                         in if c = 1 
                            then m.[row, 0]
                            else MatrixValue (array2D [| [| for i in 0..c-1 -> m.[row, i] |] |], 1, c)
                    else failwithf "Subscript index must be an integer."
                | _ -> failwithf "Subscript cannot be applied to a non-matrix value."
            in ConstantValue (c, Some (comp v))

        | Column, None, v ->
            let comp v1 v2 =
                match v1, v2 with
                | MatrixValue (m, r, c), NumValue n -> 
                    if Decimal.Round(n) = n 
                    then let col = Decimal.ToInt32(n) 
                         in if r = 1 
                            then m.[0, col]
                            else MatrixValue (array2D [| for i in 0..c-1 -> [| m.[i, col]|] |], 1, c)
                    else failwithf "Column index must be an integer."
                | _ -> failwithf "Column cannot be applied to a non-matrix value."
            in ConstantValue (c, Some (comp v))

        | _, Some f, v -> f v
        | _, _, _ -> failwithf "Cannot apply function to this type of value." // TODO: better error message 


    let rec interpret (env : Env) e =
        match e with
            | HoleExp _  -> failwithf "Cannot interpret an empty expression." 
            | NumExp n   -> NumValue n
            | ConstExp c -> 
                match c with 
                | True  -> BooleanValue true
                | False -> BooleanValue false
                | _     -> ConstantValue (c, None) 

            | FunExp (x, e') -> FunValue (x, e')

            | VarExp x ->
                match env.TryFind x with
                | Some v -> v
                | None   -> failwithf "Variable %s is undefined." x
            
            | AppExp (e1, e2) ->
                match interpret env e1 with
                | FunValue (x, e1') -> 
                    let v2 = interpret env e2 
                    in interpret (env.Add (x, v2)) e1'
                | ConstantValue (c, v') ->
                    let v2 = interpret env e2
                    in applyConstant c v' v2
                | _ -> failwithf "Cannot apply a non-function value."

            | MatrixExp (es, r, c) ->
                let rows = [| for i in 0..r-1 -> [| for j in 0..c-1 -> interpret env es.[i, j] |] |]
                in MatrixValue (array2D rows, r, c)

            | GuardExp cases ->
                match cases with
                | [] -> failwithf "Non-exhaustive guarded expression."
                | (None, body) :: _ -> interpret env body 
                | (Some guard, body ) :: cases' ->
                    match interpret env guard with
                    | BooleanValue true  -> interpret env body 
                    | BooleanValue false -> interpret env (GuardExp cases')
                    | _ -> failwithf "Cannot use non-boolean value as truth-value."


    let exampleEnv = Map.empty : Env
    let exampleAST = AppExp ((AppExp ((ConstExp Less), (NumExp 50M))), (NumExp 20M))
    let exampleAST2 = GuardExp ((Some (AppExp ((AppExp (ConstExp Land, ConstExp True)), ConstExp True)), NumExp 1337M) :: [((None : Exp Option), NumExp 420M)])
    let exampleAST3 = AppExp (AppExp (ConstExp Power, NumExp 1000M), NumExp 0.333333333M)
    let exampleAST4 = AppExp (AppExp (ConstExp Subscript, MatrixExp (array2D [| [| NumExp 1M; NumExp 2M |]; [| NumExp 3M; NumExp 4M |]; [| NumExp 5M; NumExp 6M |] |], 3, 2)), NumExp 2M)
    let exampleAST5 = AppExp (AppExp (ConstExp Multiplication, NumExp 53M), MatrixExp (array2D [| [| NumExp 1M; NumExp 2M |]; [| NumExp 3M; NumExp 4M |]; [| NumExp 5M; NumExp 6M |] |], 3, 2))
    let exampleAST6 = MatrixExp (array2D [| [| NumExp 1M; NumExp 2M |]; [| NumExp 3M; NumExp 4M |] |], 2, 2)
    let exampleAST7 = AppExp (AppExp (ConstExp Multiplication, MatrixExp (array2D [| [| NumExp 1M; NumExp 2M |]; [| NumExp 3M; NumExp 4M |]; [| NumExp 5M; NumExp 6M |] |], 3, 2)),  MatrixExp (array2D [| [| NumExp 1M; NumExp 2M; NumExp 3M; NumExp 4M; NumExp 5M |]; [| NumExp 6M; NumExp 7M; NumExp 8M; NumExp 9M; NumExp 10M |] |], 2, 5))
