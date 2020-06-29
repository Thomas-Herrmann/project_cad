module DecimalMath

open System

let iterSqrt (n : decimal) (iter : int) =
    let rec guess g i =
        match i with
        | 0 -> g
        | _ -> guess ((g + (n / g)) / 2M) (i - 1)
    in guess (n / 2M) iter


let rec precisionPower (b : decimal) (p : decimal) (precision : int) =
    if p < 0M then 1M / (precisionPower b (-p) precision)
    elif p >= 10M then let temp = precisionPower b (p / 2M) (precision / 2) in Decimal.Multiply(temp, temp)
    elif p >= 1M then Decimal.Multiply(b, (precisionPower b (p - 1M) precision))
    elif precision >= 1 then iterSqrt b 1000
    else iterSqrt (precisionPower b (p * 2M) (precision * 2)) 1000


let precisionRoot (b : decimal) (n : decimal) (precision : int) = precisionPower b (1M / n) precision 