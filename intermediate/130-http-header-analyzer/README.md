# HTTP Header Analyzer

Analyze HTTP response headers and security headers for any website.

## Usage

```bash
dotnet run --project 130-http-header-analyzer.csproj -- <url> [--head] [--no-redirect]
```

## Example

```bash
dotnet run --project 130-http-header-analyzer.csproj -- https://example.com
dotnet run --project 130-http-header-analyzer.csproj -- https://google.com --head
```

### Sample Output

```
Analyzing headers for: https://example.com
Request type: GET

=== Response Information ===
Status Code: 200 OK
Content Type: text/html; charset=UTF-8

=== Response Headers ===
Cache-Control                  : public, max-age=604800
Content-Type                   : text/html; charset=UTF-8
Server                         : ECS (dcb/7F84)

=== Security Headers Analysis ===
Present (3):
  ✓ X-Content-Type-Options - Prevents MIME type sniffing
  ✓ ...

Missing (7):
  ✗ Strict-Transport-Security - HSTS - Forces HTTPS connections
  ✗ Content-Security-Policy - CSP - Prevents XSS attacks

Security Score: 3/10 headers present
```

## Security Headers Checked

- Strict-Transport-Security (HSTS)
- Content-Security-Policy (CSP)
- X-Content-Type-Options
- X-Frame-Options
- X-XSS-Protection
- Referrer-Policy
- Permissions-Policy
- Cross-Origin policies

## Concepts Demonstrated

- HttpClient configuration
- Header collection handling
- Security header analysis
- Technology detection
- HTTP protocol understanding
