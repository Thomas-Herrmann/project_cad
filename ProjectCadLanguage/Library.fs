﻿namespace ProjectCadLanguage

open Microsoft.FSharp.Collections
open System

type Constant = True | False | Not | Less | Greater | LessEq | GreaterEq | Eq | NotEq | Land | Lor // Boolean operators
              | Plus | Minus | UnaryMinus | Multiplication | Division | Power              // Arithmetic operators
              // Matrix operators

type Exp = NumExp of decimal
         | ConstExp of Constant
         | VarExp of string
         | AppExp of Exp * Exp
         | FunExp of string * Exp
         | MatrixExp of Exp[,] * int * int
         | GuardExp of (Option<Exp> * Exp) List

type Value = NumValue of decimal
           | ConstantValue of Constant * Option<Value -> Value>
           | FunValue of string * Exp
           | MatrixValue of Value[,] * int * int
           | BooleanValue of bool

type Env = Map<string, Value>



module Language =

    let valueToString = function
        | NumValue n -> n.ToString()
        | ConstantValue (c, _) -> "TODO"
        | FunValue (x, _) -> "fun x"
        | MatrixValue (vs, _, _) -> vs.ToString()
        | BooleanValue b -> if b then "true" else "false" 

    let applyConstant c v' v2 = 
        match c, v', v2 with
        | Not, None, BooleanValue b -> BooleanValue (not b)
        | Less, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) < 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "< cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))
        | Less, Some f, v -> f v

        | Greater, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) > 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "> cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))
        | Greater, Some f, v -> f v

        | LessEq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) <= 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "<= cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))
        | LessEq, Some f, v -> f v

        | GreaterEq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) >= 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf ">= cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))
        | GreaterEq, Some f, v -> f v

        | Eq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) = 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "== cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))
        | Eq, Some f, v -> f v

        | NotEq, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | NumValue n1, NumValue n2 -> if Decimal.Compare(n1, n2) <> 0 then BooleanValue true else BooleanValue false
                | _ -> failwithf "<> cannot be applied to a non-decimal value."
            in ConstantValue (c, Some (comp v))
        | NotEq, Some f, v -> f v

        | Land, None, v ->
            let comp v1 v2 = 
                match v1, v2 with 
                | BooleanValue b1, BooleanValue b2 -> if b1 && b2 then BooleanValue true else BooleanValue false
                | _ -> failwithf "^ cannot be applied to a non-boolean value."
            in ConstantValue (c, Some (comp v))
        | Land, Some f, v -> f v

        | Lor, None, v ->
                   let comp v1 v2 = 
                       match v1, v2 with 
                       | BooleanValue b1, BooleanValue b2 -> if b1 || b2 then BooleanValue true else BooleanValue false
                       | _ -> failwithf "v cannot be applied to a non-boolean value."
                   in ConstantValue (c, Some (comp v))
        | Lor, Some f, v -> f v

        // TODO: more cases ..

        | _, _, _ -> failwithf "Cannot apply function to this type of value." // TODO: better error message 


    let rec interpret (env : Env) e =
        match e with
            | NumExp n       -> NumValue n
            | ConstExp c     -> 
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
                let rows = [for i in 0..r -> [for j in 0..c -> interpret env es.[i, j]]]
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
