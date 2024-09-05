# ClearBank.DeveloperTest
ClearBank refactoring exercise

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

### Some Notes
- I have used the test isolation framework Moq sparingly and instead favoured using Exract and Override nearly until the end where I then killed of the "TestablePaymentService" I'd created
- I considered libraries such as BDDFy and can discuss benefits of this (I have other code tests :) to show you if needs be :)) 
- Sad paths and error handling are virtually non-existent
- The Backup Data Store vs Normal Data store, prolly not great to have this only decided with a config manager var
- Thanks for the opportunity - this was good practice for me whatever happens :) 