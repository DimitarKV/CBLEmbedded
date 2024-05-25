#include <iostream>
using namespace std;
const int MAX_WEIGHT = 60;
int nextBin(char color, int firstWeight, int secondWeight, int thirdWeight)
{
    int currentWeight;
    int nextBin;
    int currentMin;

    if (color == 'b'){
        currentWeight = 20;
    } else {
        currentWeight = 10;
    }

    if ((firstWeight + currentWeight) > MAX_WEIGHT && (secondWeight+currentWeight) > MAX_WEIGHT && (thirdWeight+currentWeight) > MAX_WEIGHT) {
        //cout << "Impossible operation" << endl;
        return -1;
    }

    currentMin = firstWeight;
    nextBin = 0;

    if (currentMin > secondWeight){
        currentMin = secondWeight;
        nextBin = 1;
    }

    if (currentMin > thirdWeight){
        currentMin = thirdWeight;
        nextBin = 2;
    }

    return nextBin;
}
int main()
{
    char color;
    int answer;
    int currentWeights[3];
    cin >> color >> currentWeights[0] >> currentWeights[1] >> currentWeights[2];
    answer = nextBin(color, currentWeights[0], currentWeights[1], currentWeights[2]);
    cout << answer << endl;
    return 0;
}
