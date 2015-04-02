Emkay.S3
========

This package contains MSBuild tasks for Amazon S3, which provides the possibility to upload files, enumerate buckets, enumerate the content of a specific 'subfolder', delete buckets and delete files from specific subfolders.

##Download

The Emkay.S3 library is available on nuget.org via package name Emkay.S3.

To install it, run the following command in the Package Manager Console

	PM> Install-Package Emkay.S3

More information about NuGet package avaliable at [https://nuget.org/packages/Emkay.S3](https://nuget.org/packages/Emkay.S3)

##Getting Started

TODO: Bit on installing via nuget at build time 

In order to use the tasks in your project, you need to import the Emkay.S3.Tasks.targets file. Maybe you need to adjust the paths to your needs.

    <Import Project="Emkay.S3.Tasks.targets"/>

Emkay S3 file publisher is an **MSBuild task** which can be used for publishing a file or set of files to S3. By default the files will be publically accessible. The following target will upload `file.txt` to `path/within_S3/file.txt`:

    <Target Name="S3_upload">
      <Message Text="Publishing to S3 ..." />
      
      <Message Text="Source folder: $(source)"/>
      <Message Text="Bucket: $(S3_bucket)"/>
      <Message Text="Destination folder: $(S3_subfolder)"/>
      
      <PublishFiles
        Key="$(aws_key)"
        Secret="$(aws_secret)"
        SourceFiles="path\to\file.txt"
        Bucket="my_s3_bucket"
        DestinationFolder="path/within_S3" />


  	</Target>


## Examples

### Uploading a folder

You can also recursively publish an entire folder:

      <ItemGroup>
        <UploadFiles Include="localpath\**\*.*" />
      </ItemGroup>
      <PublishFiles
        Key="$(aws_key)"
        Secret="$(aws_secret)"
        SourceFiles="@(UploadFiles)"
        Bucket="$(aws_s3_bucket)"
        DestinationFolder="path/within_S3" />


### Headers

You can set custom headers by using ItemGroup metadata:

      
      <ItemGroup>
        <UploadFiles Include="localpath\**\css\**\*.css">
          <Content-Type>text/css</Content-Type>
          <Content-Encoding>gzip</Content-Encoding>
        </UploadFiles>
        <UploadFiles Include="localpath\**\**\*.json" Exclude="@(UploadFiles)">
          <Content-Type>application/json</Content-Type>
        </UploadFiles>
        <UploadFiles Include="localpath\**\*.*" Exclude="@(UploadFiles)" />
      </ItemGroup>


Note that only [headers supported by Amazon S3](http://docs.aws.amazon.com/AmazonS3/latest/API/RESTObjectPUT.html#RESTObjectPUT-requests-request-headers) will be respected. The main ones to be concerned with:

 * `Cache-Control`: Can be used to specify caching behavior along the request/reply chain. For more information, go to http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.9.
 * `Content-Disposition`: Specifies presentational information for the object. For more information, go to http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.5.1.
 * `Content-Encoding`: Specifies what content encodings have been applied to the object and thus what decoding mechanisms must be applied to obtain the media-type referenced by the Content-Type header field. For more information, go to http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.11.
 * `Content-Type`: A standard MIME type describing the format of the contents. For more information, go to http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.17.
 * `Expires`: The date and time at which the object is no longer cacheable. For more information, go to http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.21.
 * `x-amz-meta-`: Any header starting with this prefix is considered user metadata. It will be stored with the object and returned when you retrieve the object. The PUT request header is limited to 8 KB in size. Within the PUT request header, the user-defined metadata is limited to 2 KB in size. User-defined metadata is a set of key-value pairs. The size of user-defined metadata is measured by taking the sum of the number of bytes in the UTF-8 encoding of each key and value.
 * `x-amz-storage-class`: RRS enables customers to reduce their costs by storing noncritical, reproducible data at lower levels of redundancy than Amazon S3's standard storage. Valid Values: `STANDARD`, `REDUCED_REDUNDANCY`
 * `x-amz-website​-redirect-location`: If the bucket is configured as a website, redirects requests for this object to another object in the same bucket or to an external URL. Amazon S3 stores the value of this header in the object metadata. For information about object metadata, go to [Object Key and Metadata](http://docs.aws.amazon.com/AmazonS3/latest/dev/UsingMetadata.html).  The value must be prefixed by, `/`, `http://` or `https://`. The length of the value is limited to 2 KB.
 
## Help

All tasks have some common properties:

* *Key*: Required. Your AWS key. Normally you should use a variable, and pass the value as a parameter to MSBuild or read it from somewhere outside of source control (a file or registry).
* *Secret*: Required. Your AWS secret. Normally you should use a variable, and pass the value as a parameter to MSBuild or read it from somewhere outside of source control (a file or registry).
* *Bucket*: Required. The AWS S3 bucket you are operating on.
* *Region*: The AWS S3 region to use. By default "us-east-1" is used, but any of the [AWS S3 region names](http://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region) can be used.

TODO: document each individual task

## License
The source code is available under the [MIT license](http://opensource.org/licenses/mit-license.php).

