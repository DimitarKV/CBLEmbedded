#include <iostream>
using namespace std;
const int MAX_WEIGHT = 40;
int currentWeights[3] = {0, 0, 0};
bool flag = true;

void nextBin(char color)
{
    int currentWeight;
    int nextBin;
    int currentMin;

    if (color == 'b'){
        currentWeight = 20;
    } else {
        currentWeight = 10;
    }

    if ((currentWeights[0] + currentWeight) > MAX_WEIGHT && (currentWeights[1]+currentWeight) > MAX_WEIGHT && (currentWeights[2]+currentWeight) > MAX_WEIGHT) {
        cout << "Impossible operation" << endl;
        flag = false;
        return;
    }

    currentMin = currentWeights[0];
    nextBin = 0;
    if (currentMin > currentWeights[1]){
        currentMin = currentWeights[1];
        nextBin = 1;
    }

    if (currentMin > currentWeights[2]){
        currentMin = currentWeights[2];
        nextBin = 2;
    }

    currentWeights[nextBin] += currentWeight;
    cout << currentWeights[0] << " " << currentWeights[1] << " " << currentWeights[2] << endl;


}
int main()
{
    char color;
    while(flag)
    {
        cin >> color;
        nextBin(color);
    }
    return 0;
}

