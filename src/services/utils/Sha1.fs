module MyTeam.Sha1

open System.Security.Cryptography
open System.Text



let private hexStringFromBytes bytes =
     
    let sb = new StringBuilder()
    for (b: byte) in bytes do   
        let hex = b.ToString("x2")
        sb.Append(hex) |> ignore
    
    sb.ToString()
        


let hashStringForUTF8String (s: string) =
        
        let bytes: byte[] = Encoding.UTF8.GetBytes(s)

        let sha1 = SHA1.Create()
        let hashBytes = sha1.ComputeHash(bytes)

        hexStringFromBytes(hashBytes)
        
