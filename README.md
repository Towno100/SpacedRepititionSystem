# SpacedRepititionSystem
A Spaced Repitition System Library loosely based on Leitner system 

## Review Engine
Takes a collection of data which implements IReviewable and can select which items to review next and parse test results
The ReviewManager exposes two methods:
1. SetCurrent - Fills bucket of current study items to the desired number
2. ParseTestResults - Takes test items, correct results, and a function to determine if answer is correct

## Quickstart
A simple Console project is included. Here's how to run it:
1. Build it, and run the EF Core command Update-Database 
2. Execute the included SQL script to seed the data
3. Run the project

Enjoy!


