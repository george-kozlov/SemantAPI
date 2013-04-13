##SemantAPI Sentiment Analysis toolkit##

**SemantAPI** is a free, open source toolkit intended for easy and fast sentiment analysis on the most popular NLP solutions.
Toolkit offers 2 independnt analysis applications: SemantAPI.Robot and SemantAPI.Human.
Both applications are written on C# and based on Microsoft' .Net framework 3.5 platform.

**SemantAPI.Robot** is a robotic application that takes specified source file and run ana;ysis of every line on selected services.
As result application generates regular CSV file with the two columns per selected service.
"Sentiment score" column contains float sentiment value provided by the target service that can be used for precise sentiment analysis if necessarry.
In oposite "Sentiment polarity" value contains verbal representation of the sentiment score that is easy to read and understand for humans.
Current version of SemantAPI.Robot application supports following NALP solutions:
- [Semantria](https://semantria.com). Modern, fast-growing NLP solution based on [Lexalytics’ Salience](http://www.lexalytics.com/technical-info/salience-engine-for-text-analysis) engine.
- [AlchemyAPI](http://www.alchemyapi.com). Seems on of the world's most popular NLP solution.
- [Chatterbox](http://chatterbox.co). The social technology engine that uses machine learning for sentiment analysis.
- [Viralheat](https://www.viralheat.com). Social media monitorin solution that offers sentiment analysis API for 3rd-party integrators.
- [Bitext](http://www.bitext.com). Semantic technologies solution with sentiment analysis API that claims about \*highest accuracy in the market.

**SemantAPI.Human** is an utomatic application that allows adding of human judge sentiment polarity using Amazon' [Mechanical Turk](https://www.mturk.com/mturk/welcome) service.
Application uses output of SemantAPI.Robot tool and ads Mechanical Turk output to the same CSV file as a gold reference for sentiment analysis accuracy.
No one can rely on a human judge because it can be too subjective but there is no better way to get better reference for the sentiment than human score.
Application offers set of options for precise configuration of Mechanical Turk HITs. Number of assihnments and reward amount are the most important of them.
Out of the box the tool requests analysis of each document 3 times and finally calculates the average sentiment polarity based on human score.
"MechanicalTurk Score" column represents a confidence score related to the provided "Sentiment Polarity".
So, if two people said that sentiment is negative but one person said positive, tool will return Negative "Sentiment Polarity" with 0.67(2/3) confidence score. 

   \* I'm not sure about accuracy, but their API is the worst among the integrated.