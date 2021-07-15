namespace MyTeam

open Microsoft.Extensions.Options
open Giraffe
open Shared
open Shared.Image
open System
open Giraffe.ViewEngine


module Images =
    let getSecretOptions (ctx: HttpContext) =
        (ctx.GetService<IOptions<CloudinarySettings>>())
            .Value

    let getOptions (ctx: HttpContext) =
        getSecretOptions ctx
        |> fun opts ->
            { CloudName = opts.CloudName
              DefaultMember = opts.DefaultMember =?? ""
              DefaultArticle = opts.DefaultArticle =?? "" }

    let get ctx url getProps =
        let options = getOptions ctx
        Image.get options url getProps

    let getArticle ctx (getProps: GetProps) url =
        let options = getOptions ctx
        Image.getArticle options getProps url

    let getMember ctx (getProps: GetProps) url facebookId =
        let options = getOptions ctx
        Image.getMember options getProps url facebookId


    let uploadScripts (options: CloudinarySettings) =

        let unixTimestamp =
            int
                (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)))
                    .TotalSeconds

        let queryString =
            sprintf "timestamp=%i%s" unixTimestamp options.ApiSecret

        let signature = Sha1.hashStringForUTF8String queryString

        [ script [ _src "/cloudinary.bundle.js" ] []
          script [] [

              rawText
              <| sprintf
                  "$.cloudinary.config({ cloud_name: '%s', api_key: '%s', signature: '%s' });

                                        var formData = {
                                            api_key: '%s',
                                            timestamp: '%i',
                                            signature: '%s'
                                        }

                                        $('.cloudinary-fileupload').each(function () {
                                            $(this).attr('data-form-data', JSON.stringify(formData));
                                        });

                                        $('.cloudinary-fileupload').bind('cloudinarydone', function (e, data) {
                                            $('.cloudinary-preview').html(
                                              $.cloudinary.image(data.result.public_id,
                                                {
                                                    format: data.result.format, version: data.result.version,
                                                    crop: 'fill'
                                                })
                                            );
                                            $('input[name=ImageFull]').val('image/upload/' + data.result.path);
                                            return true;

                                        });

                                        $('.cloudinary-fileupload').bind('fileuploadprogress', function (e, data) {
                                            $('.cloudinary-progress-bar').css('width', Math.round((data.loaded * 100.0) / data.total) + '%%');
                                        });"
                  options.CloudName
                  options.ApiKey
                  signature
                  options.ApiKey
                  unixTimestamp
                  signature

          ] ]
