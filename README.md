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

    <PublishFiles
      Key="$(aws_key)"
      Secret="$(aws_secret)"
      Region="eu-central-1"
      Bucket="my_s3_bucket"
      SourceFiles="path\to\file.txt"
      DestinationFolder="path/within_S3" />

You can easily recursively publish an entire folder:

    <ItemGroup>
      <UploadFiles Include="localpath\**\*.*" />
    </ItemGroup>
    <PublishFiles
      Key="$(aws_key)"
      Secret="$(aws_secret)"
      Region="us-east-1"
      Bucket="my_s3_bucket"
      SourceFiles="@(UploadFiles)"
      DestinationFolder="path/within_S3" />

In this example, a local file `localpath\subdir1\subdir2\file.txt` would be uploaded to `path/within_s3/subdir1/subdir2/file.txt`. See the PublishFiles help below for more detail on source and destination paths.

## Tasks

All tasks have some common properties:

* **Key**: Required. Your AWS key. Normally you should use a variable, and pass the value as a parameter to MSBuild or read it from somewhere outside of source control (a file or registry).
* **Secret**: Required. Your AWS secret. Normally you should use a variable, and pass the value as a parameter to MSBuild or read it from somewhere outside of source control (a file or registry).
* **Region**: The AWS S3 region to use. By default "us-east-1" is used, but any of the [AWS S3 region names](http://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region) can be used.

### PublishFiles

Alias: `PublishFilesWithHeaders`

Additional Properties:

 * **Bucket**: Required. The AWS S3 bucket you are operating on.
 * **DestinationFolder**: Required (but can be blank to indicate root). The destination folder within the S3 bucket to place uploaded files.
 * **SourceFiles**: Required. The ItemGroup containing the list of files to upload. 
 * **PublicRead**: If the files are accessible to Everyone. Defaults to False. 
 * **FlattenFolders**: If the original source path is ignored when determining the target S3 folder. By setting to True, all files are placed in the DestinationFolder, regardless of their source location.

The ultimate destination of a file is determined by the DestinationPath, SourceFiles path, and the value of FlattenFolders. 

DestinationPath is always used as the base folder for all uploaded files. 

If FlattenFiles is true, then all files are put directly into DestinationPath, regardless of their source location. If FlattenFiles is flase, then the source path of the file will be appended to the DestinationPath. If the SourceFiles path contains a wildcard, only the recursive portion of the path is used. 

#### Headers

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


#### Example 1

For example, given the structure:

 * `files\file1.txt`
 * `files\subdir1\file2.txt`

If the following task is executed:

    <PublishFiles Key=".." Secret=".."
      SourceFiles="files\file1.txt;files\subdir1\file2.txt"
      DestinationFolder="uploadedfiles" />
      
Then the files `uploadedfiles/files/file1.txt` and `uploadedfiles/files/subdir1/files2.txt` will be created.

#### Example 2 

If the following task is executed:

    <ItemGroup>
      <ExampleFiles2 Include="files\*.txt;files\subdir1\*.txt"
    </ItemGroup>
    <PublishFiles Key=".." Secret=".."
      SourceFiles="@(ExampleFiles2)"
      DestinationFolder="uploadedfiles" 
      FlattenFolders="True" />
      
Then the files `uploadedfiles/file1.txt` and `uploadedfiles/files2.txt` will be created.

In the case of conflicts, the files will be overwritten with the last one specified by the include. 

#### Example 3

If the following task is executed:

    <ItemGroup>
      <ExampleFiles2 Include="files\**\*.*"
    </ItemGroup>
    <PublishFiles Key=".." Secret=".."
      SourceFiles="@(ExampleFiles2)"
      DestinationFolder="uploadedfiles" />
      
Because of the wildcard specification, the `files/` prefix will not be included, and so the files `uploadedfiles/file1.txt` and `uploadedfiles/subdir1/files2.txt` will be created.

### DeleteBucket

Deletes a bucket and its contents.

Additional Properties:

 * **Bucket**: Required. The AWS S3 bucket to delete.
 
#### Example
 
     <DeleteBucket Key=".." Secret=".."
       Bucket="bucket_to_delete" />

### DeleteChildren

Additional Properties:

 * **Bucket**: Required. The AWS S3 bucket you are operating on.
 * **Children**: Required. The list of children items to delete from S3. 

TODO: Examples

### EnumerateBuckets

Outputs:
 * **Buckets**: The list of buckets for this account.

#### Example

    <EnumerateBuckets Key=".." Secret="..">
      <Output TaskParameter="Buckets" ItemName="FoundBuckets" />
    </EnumerateBuckets>
    <Message Text="Found bucket: %(FoundBuckets.identity)" />

### EnumerateChildren

Additional Properties: 

 * **Bucket**: Required. The AWS S3 bucket you are operating on.
 * **Prefix**: Only include children with this prefix.
 
Outputs:

 * **Children**: The list of children objects

#### Example

    <EnumerateChildren Key=".." Secret=".."
      Bucket="my_s3_bucket"
      Prefix="myfolder/">
      <Output TaskParameter="Children" ItemName="Files" />
    </EnumerateChildren>
    <Message Text="Found file: %(Files.identity)" />


## License
The source code is available under the [MIT license](http://opensource.org/licenses/mit-license.php).

