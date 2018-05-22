# Email Service Test

### Problem
This current implementation of an email service works most of the time. Callers of this service have implemented retry mechanisms to get their email sent. In the production environment, the service has been observed failing with one of two types of failures. 

The first is a `Connection Failed` exception. This occurs randomly and users are complaining they must implement retry mechanisms and try many times.

The second exception is a `Unexpected Error` exception. Whenever this exception shows up in the logs, the service crashes and needs to be started again.

Finally, a request has come in to begin accepting post requests on a webhook endpoint. The service utilizing this endpoint needs HTTP responses provided to decide what to do.

### Outline

These problems should be rewritten, if possible, into failing tests replicating the reported failures. The tests may be integration or unit tests. Once these tests are in place, code modifications to make the tests pass are expected.

The included MockEmailClient is not to be modified.

### Running the solution

First you need a working installation of dotnet core https://www.microsoft.com/net/learn/get-started/windows

To get the API up and running, the following dotnet command line command should start the project:
```
dotnet run --project EmailService
```

To run the tests:
```
dotnet test EmailServiceTests
```

A dotnet geared IDE may assist further with debugging.

### Submission
Once completed, please have the code available for your technical interview as a fork of this project.
