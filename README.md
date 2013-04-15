##SemantAPI Sentiment Analysis toolkit##

**SemantAPI** is a free, open source toolkit intended for a quick and easy comparison of the most popular NLP and sentiment analysis solutions on the market. The toolkit offers 2 independent analysis applications: SemantAPI.Robot and SemantAPI.Human. Both applications are written in C# and based on Microsoft's .Net framework 3.5 platform.

**SemantAPI.Robot** is an application that takes the specified source file and runs an analysis of every line therein, using the selected services. 

The results are generated in a regular CSV file, with two columns per selected service:
- The "sentiment score" column contains float sentiment values provided by the target service, which can be used for precise sentiment analysis. 
- The "sentiment polarity" value contains a verbal representation of the sentiment score, making it easy to read and understand at a glance.

The current version of the SemantAPI.Robot application supports the following NLP solutions:
- [Semantria](https://semantria.com). Modern, fast-growing NLP solution based on [Lexalytics' Salience](http://www.lexalytics.com/technical-info/salience-engine-for-text-analysis) engine.
- [AlchemyAPI](http://www.alchemyapi.com). One of the world's most popular NLP solutions.
- [Chatterbox](http://chatterbox.co). Social technology engine that uses machine learning for sentiment analysis.
- [Viralheat](https://www.viralheat.com). Social media monitoring solution that offers a sentiment analysis API for 3rd-party integrators.
- [Bitext](http://www.bitext.com). Semantic technologies solution with a sentiment analysis API that claims to have the highest accuracy on the market.

**SemantAPI.Human** is an application that uses Amazon's [Mechanical Turk](https://www.mturk.com/mturk/welcome) service, to have humans assign a sentiment score to submitted documents. The application uses the same output as the SemantAPI.Robot tool, and adds the Mechanical Turk output to the same CSV file as the baseline reference for sentiment analysis accuracy. The problem with human scoring is that it's very subjective (trained humans only agree on sentiment scores 80% of the time at best), but it's still the best option available as a reference. 
The SemantAPI.Human application offers a variety of options to precisely configure the Mechanical Turk [HITs] (https://www.mturk.com/mturk/). The number of assignments and the reward amount are the most important options. Out of the box, the tool requests an analysis of each document 3 times and finally calculates the average sentiment score and polarity based on the humans' score. The "MechanicalTurk Score" column represents a confidence score related to the provided "Sentiment Polarity". So, if two people said that the sentiment is negative but one person said positive, the tool will return a Negative "Sentiment Polarity" with 0.67(2/3) confidence score.
