# FluentFTP Migration Plan for FtpFileSystemProviderService

## 1. Add FluentFTP NuGet Package

- Add `FluentFTP` to the project via NuGet.

## 2. Refactor FtpFileSystemProviderService

- Replace all usages of `FtpWebRequest` and related types with `FluentFTP`'s `FtpClient`.
- Update all FTP operations (list, download, upload, delete, etc.) to use FluentFTP async APIs.
- Remove obsolete code and error handling patterns.
- Update constructor to inject/configure `FtpClient` as needed.

## 3. Update Configuration

- Map `FtpFileSystemOptions` to FluentFTP's connection options.

## 4. Update Logging

- Integrate FluentFTP's logging with Microsoft.Extensions.Logging if needed.

## 5. Test and Validate

- Ensure all methods work as expected and pass unit/integration tests.

---

**Next Steps:**

1. Add FluentFTP NuGet package to the project.
2. Refactor `FtpFileSystemProviderService` to use FluentFTP.
3. Remove all obsolete `FtpWebRequest` code.
4. Test and verify.
