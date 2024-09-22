# ClearBank.DeveloperTest
ClearBank refactoring exercise

## EDIT: Extra notes
1.  This submisson was a *fail*, use it as an example of what not to do.
2. As a refactor and not a redesign my approach was to first methodically add test coverage, sociably first and then begin to refactor.  I spent more effort on adding tests including adding a testable sut and eventually removing this, than on the consequent refactor. 
3. I stopped refactoring when I reached about 3 hours of effort. The refactor I have done, explicitly organises the implementation to expose certain characteristics of SOLID. SRP, Open Closed, Dependency Inversion in particular. BUT actually some of the dependencies might not need to exist, internal implementations could remain so unless extra testing or/usages were needed for sad paths or where behaviour is not easily executable from the public API entry point.
4. Before concluding the refactor, I did miss some tests paths when not using Rider code coverage tool properly. Simple to rectify and add a few more additional tests (think missing range checks)
5. The interface files could be squashed into the impl files
6. The tests could be organised by file and by scenarios
7. The factory which picks out the datastore could actually be removed and an alternative would be to inject the datastore and this would become a decision at root composition time based on config, the choice would therefore be moved up a level and defined by the host. Yet another alternative may be to introduce something like Polly and on failure of using the normal datastore instead use the fallback datastore.  Arguably the way the logic is now could mean that choosing to be more or less tolerant of failure becomes the purpose of the flag in configuration.
Yet another alternative is to have a variation of above and host in process fronted by a load balancer of some descripton the normal and fallback variants. A healthcheck could then add and remove these from the lb set. It adds some redundancy but enabling retrying using queuing may also be another approach to help smooth out transient issues.
8. I did not add validation, consider this 
9. The Dtos are used as is, this may be a consideration
10. Account verification could be encapsulated on the account model
11. If anyone passes this test at L5, lead level, please let me know what was required for my learning.

### Executing Tests
Please run:

```dotnet clean```

```dotnet build```

```dotnet test```

### Approach
Get some tests around it all... :) ,  

1 approach I have used to characterise legacy/unknown code is to get tests around the entire lot and ONLY then -  start refactoring, as taught by Michael Feathers and Sandro Mancuso
The default datastore (e.g. not the Backup one) is on the shallowest code branch, so tests are added here to be added here initially. Seems a good as place as any to start off...

It is immediately obvious this code/service is hard to test and has hardcoded dependencies, which will be addressed in due course
 Boiler Plate commits…and then…

1. Commit 1 - is simply getting a test to execute around the smallest branch, with a known result  - a fail - can see immediately we will need to invert configuration at some point and there are dependencies which will need to be extracted
2. Commit 2 - Bacs account testing,  introduce a seam in TestablePaymentService - so we can start controlling the SUT.  I use extract and override to return an account which can give us a Successful result  -  A Bacs account in this instance
3. Commit 3 -Faster Payments account testing, Looking at next code branch, modifying both the request to add an amount, and allowing the account balance to have enough funds to continue to allow successful payment - TODO - tests - thinking about a builder and asserting balance adjustments,
4. Commit 4 - Chaps payments testing, Refactor to introduce builder to help with explicitness and an element of DRY for SUT set up - not perfect but will help limit changes in tests as the TestablePaymentService is adjusted
5. Commit 5 - Now dig a bit deeper  branch wise and check the command to update the datastore is receiving the correct debited amount.  A bit of a sin is committed as we add a spy to to the TestablePaymentService, but it doesn’t look very nice. This is done for Bacs First.   The sense that duplication owing to the different AccountStore types will occur in the Builder shortly (which I can see in the distance needed to be abstracted behind a common interface)
6. Commit 6 - Faster payments addressed for checking balance

Backup data store is now tested specifically
1. Commit 7 - A seam is now added for the configuration used to drive data store selection, to get it testable and we can start exercising happy paths with the backup data store - the refactor is not far away…
2. Commit 8 - Refactor tests so backup and non-backup datastore paths are exercised more easily, DRYs up tests a bit
3. Commit 9 - Add null account check for completeness, thought there may be more sad tests

Now Refactor from the deepest code branch to pull out all the dependencies....
1. Commit 10 -  First extraction of request payment scheme eligibility checking. Better but not perfect
2. Commit 11 -  Refactor to improve naming, move files about for eligibility checking
3. Commit 12 -  A Bit more refactoring  - now moving back to service to refactor AccountDataStore selection
4. Commit 13 - Inverted dependencies - added test where coverage was missing - Some isolated unit tests but aim is to be as sociable as possible to avoid this - for happy path at least…
5. Commit 14 - Had a play with moving balance update to AccountService - perhaps this is too much

Tidy up
6. Commit 15 - Some refactoring and renaming, realised call to "command" wasn't verified

### Some Notes
- I have used the test isolation framework Moq sparingly and instead favoured using Exract and Override nearly until the end where I then killed of the "TestablePaymentService" I'd created
- I considered libraries such as BDDFy and can discuss benefits of this (I have other code tests :) to show you if needs be :))
- Sad paths and error handling are virtually non-existent
- The Backup Data Store vs Normal Data store, prolly not great to have this only decided with a config manager var
- Following Fowler's microservices testing then the Data Store would likely have tight integration tests, and the Payment Service exercised via a Component Test.
- There is more than 1 assert per test case. I try to limit these to assertions related to the behaviour under test.
- Thanks for the opportunity - this was good practice for me whatever happens :)
