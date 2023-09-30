#include <iostream>
#include <time.h>
#include <iomanip>
#include <fstream>
#include <math.h>
#include <vector>
#include <string>
#include <set>
#include <algorithm>
#include <climits>
#include <map>
#include <queue>

#define SP pair<vector<int>,vector<int> >
using namespace std;

// SA parameters
#define ORIGIN_T    100
#define ORIGIN_K    700
#define TIMES_RATE  12
#define TIME_LIMIT  1780
#define LIMIT_TEMP  0.05
#define DOWN_T_RATE 0.95

// Module structure
struct Module
{
    string name;
    short id;
    int w, h, x, y;
    int minArea;
    bool isFixed;
};

void initWH(const int& area, int& w, int& h);
void reshapeModule(Module& m, const double& aspectRatio);
bool readFile(vector<Module>& modules);
bool testOverlappingAll(const vector<Module>& modules, const Module& m);
double calculateHPWL(const vector<Module>& modules);
void outputFile(const vector<Module>& modules);
double calculateOutChipArea(const vector<Module>& modules);
double singleOutChipArea(const Module& m);
double findIdealPosY(Module& module, vector<Module>& modules);
double findIdealPosX(Module& module, vector<Module>& modules);
bool comparePairs(const std::pair<int, int>& p1, const std::pair<int, int>& p2);
void moveModulesY(double y, Module& module, vector<Module>& modules);
void moveModulesX(double x, Module& module, vector<Module>& modules);
vector<int> getModuleBoundary(const Module& module, const vector<Module>& modules);
vector<pair<int, int> > getCanPutPos(const set<int>& stopPointX, const set<int>& stopPointY, Module module, const vector<Module> modules);
pair<int, int> findBestPos(const vector<pair<int, int> > pos, const Module& module, vector<Module> modules);
void debugOutput(int n, time_t& t, double& acc, double& temp, double& curr_HPWL, double& best_HPWL);
bool readOutputFile(double& modules);
bool Solve(vector<Module>& modules, SP& sp, int& area);
void SP_EVAL_ORIG(const SP& sp, vector<Module>& modules);
void LCS_ORIG(const vector<int>& X, const vector<int>& Y, vector<int>& postions, const vector<int>& weights);
void getNext(vector<Module>& modules, SP& sp);
int findSequenceIndex(const vector<int>& v, const int& n);
queue<vector<Module> > MinHPWLSAWithSolveSA(vector<Module>& modules, SP& sp);
queue<vector<Module> > MinHPWLSAWithSolve(vector<Module>& modules, SP& sp);
bool SolveSA(vector<Module>& modules, SP& sp, const double& T);
bool FirstSolSA(vector<Module>& modules, SP& sp);
void setModulesWH(vector<Module> modules, const int step);
int getOverlapAreaWithFixedModules(const Module& m1, const vector<Module>& modules);
bool Solve(vector<Module>& modules, SP& sp);
void getFirst(const vector<Module>& modules, SP& sp);
vector<Module> moveModules(queue<vector<Module> > modulesQueue);
void determineDirection(vector<Module>& modules);
double isOutChip(const vector<Module>& modules);
bool isOverlapAreaWithFixedModules(const Module& m1, const vector<Module>& modules);
bool isSingleOutChipArea(const Module& m);
double getWhiteSpacePercentage(vector<Module>& modules);

// Global variables
vector<vector<int> > connections;
time_t startTime;
int chipW, chipH;
vector<vector<pair<int, int> > > modulesSize;
int softmodulesCount;
// I/O file name
string INPUT_FILE_NAME = "";
string OUTPUT_FILE_NAME = "";
int modulesCount;
bool Left = true, Down = true;

int main(int argc, char* argv[])
{
    startTime = time(NULL);
    INPUT_FILE_NAME = argv[1];
    OUTPUT_FILE_NAME = argv[2];
    vector<Module> modules;
    if (!readFile(modules))
    {
        cout << "Failed to open file.\n";
        return -1;
    }
    SP sp;
    setModulesWH(modules, 20);
    getFirst(modules, sp);
    if (FirstSolSA(modules, sp))
    {
        queue<vector<Module> > moduleQueue;
        if (getWhiteSpacePercentage(modules) <= 0.1)
            moduleQueue = MinHPWLSAWithSolveSA(modules, sp);
        else
            moduleQueue = MinHPWLSAWithSolve(modules, sp);
        modules = moveModules(moduleQueue);
    }
    outputFile(modules);
    cout << calculateHPWL(modules) << endl;
    return 0;
}

double getWhiteSpacePercentage(vector<Module>& modules)
{
    int totalArea = 0;
    for (int i = 0; i < modulesCount; i++)
    {
        if (modules[i].isFixed)
            totalArea += modules[i].w * modules[i].h;
        else
            totalArea += modules[i].minArea;
    }
    return 1 - (double)totalArea / (chipH * chipW);
}

void setModulesWH(vector<Module> modules, const int step)
{
    modulesSize.clear();
    const double ratio = 1 / (double)step;
    for (int i = 0; i < softmodulesCount; ++i)
    {
        vector<pair<int, int> >moduleSize;
        for (int j = 0; j <= step; ++j)
        {
            double acceptRatio = 1 + j * ratio;
            reshapeModule(modules[i], acceptRatio);
            if ((double)modules[i].w / modules[i].h <= 2)
                moduleSize.emplace_back(make_pair(modules[i].w, modules[i].h));
        }
        int count = moduleSize.size();
        for (int j = 1; j < count; ++j)
            moduleSize.emplace_back(make_pair(moduleSize[j].second,
                moduleSize[j].first));
        modulesSize.emplace_back(moduleSize);
    }
}

void getFirst(const vector<Module>& modules, SP& sp)
{
    for (int i = 0; i < modulesCount; ++i)
    {
        sp.first.emplace_back(i);
        sp.second.emplace_back(i);
    }

    for (int i = 0; i < modulesCount; ++i)
    {
        swap(sp.first[rand() % sp.first.size()], sp.first[rand() % sp.first.size()]);
        swap(sp.second[rand() % sp.second.size()], sp.second[rand() % sp.second.size()]);
    }
}

bool FirstSolSA(vector<Module>& modules, SP& sp)
{
    int area;
    if (Solve(modules, sp, area))
        return true;
    double T = 100;
    const double k = 1000;
    static int t = 10;
    vector<Module> bestModules = modules;
    SP bestSp = sp;
    int bestArea = area;
    while (T > 1)
    {
        int count = 0;
        modules = bestModules;
        area = bestArea;
        sp = bestSp;
        while (count++ < modulesCount * t)
        {
            vector<Module> newModules = modules;
            SP newSp = sp;
            getNext(newModules, newSp);
            int newArea;
            if (Solve(newModules, newSp, newArea))
            {
                modules = newModules;
                sp = newSp;
                return true;
            }
            double delta = (double)(newArea - area) / area;
            if (delta < 0)
            {
                modules = newModules;
                sp = newSp;
                area = newArea;
                if (newArea < bestArea)
                {
                    bestModules = newModules;
                    bestArea = newArea;
                    bestSp = newSp;
                }
            }
            else
            {
                double rrr1 = exp(-delta * k / T);
                double rrr2 = (double)rand() / RAND_MAX;
                if (rrr2 < rrr1)
                {
                    modules = newModules;
                    sp = newSp;
                    area = newArea;
                }
            }
        }
        T *= 0.95;
    }
    modules = bestModules;
    sp = bestSp;
    return false;
}

void getNext(vector<Module>& modules, SP& sp)
{
    int n = rand() % 4;
    if (n == 0)
    {
        int a = rand() % modulesCount;
        int b;
        do
        {
            b = rand() % modulesCount;
        } while (b == a);

        if (rand() % 2 == 0)
            swap(sp.first[a], sp.first[b]);
        else
            swap(sp.second[a], sp.second[b]);

        return;
    }
    else if (n == 1)
    {
        int a = rand() % modulesCount;
        int b;
        do
        {
            b = rand() % modulesCount;
        } while (b == a);

        swap(sp.first[a], sp.first[b]);
        swap(sp.second[findSequenceIndex(sp.second, sp.first[a])],
            sp.second[findSequenceIndex(sp.second, sp.first[b])]);
        return;
    }
    else
    {
        int a = rand() % softmodulesCount;
        int aspectRatio;
        do
        {
            aspectRatio = rand() % modulesSize[a].size();
        } while (modules[a].w == modulesSize[a][aspectRatio].first &&
            modules[a].h == modulesSize[a][aspectRatio].second);

        modules[a].w = modulesSize[a][aspectRatio].first;
        modules[a].h = modulesSize[a][aspectRatio].second;
        return;
    }
}

int findSequenceIndex(const vector<int>& v, const int& n)
{
    for (int i = 0; i < v.size(); ++i)
    {
        if (v[i] == n)
            return i;
    }
    return -1;
}

bool SolveSA(vector<Module>& modules, SP& sp, const double& T)
{
    int area;
    if (Solve(modules, sp, area))
        return true;
    const double k = ORIGIN_K;
    vector<Module> bestModules = modules;
    int count = 0;
    while (count++ < modulesCount)
    {
        vector<Module> newModules = modules;
        SP newSp = sp;
        getNext(newModules, newSp);
        int newArea;
        if (Solve(newModules, newSp, newArea))
        {
            modules = newModules;
            sp = newSp;
            return true;
        }
        double delta = (double)(newArea - area) / area;
        if (delta < 0)
        {
            modules = newModules;
            sp = newSp;
            area = newArea;
        }
        else
        {
            double rrr1 = exp(-delta * k / T);
            double rrr2 = (double)rand() / RAND_MAX;
            if (rrr2 < rrr1)
            {
                modules = newModules;
                sp = newSp;
                area = newArea;
            }
        }
    }
    return false;
}

queue<vector<Module> > MinHPWLSAWithSolve(vector<Module>& modules, SP& sp)
{
    double T = 100;
    const double k = ORIGIN_K;
    double HPWL = calculateHPWL(modules);

    double bestHPWL = HPWL;
    vector<Module> bestModules = modules;
    SP bestSp = sp;

    queue<vector<Module> > bestModulesQueue;
    for (int i = 0; i < 10; ++i)
        bestModulesQueue.push(modules);
    int c = 0;
    while (T > 0.05)
    //while (time(NULL) - startTime < TIME_LIMIT)
    {
        modules = bestModules;
        HPWL = bestHPWL;
        sp = bestSp;
        int initTime = time(NULL);
        c = 0;
        while (++c < modulesCount * 10)
        //while (time(NULL) - initTime < TIMES_RATE && time(NULL) - startTime < TIME_LIMIT)
        {
            vector<Module> newModules(modules);
            SP newSp(sp);
            getNext(newModules, newSp);
            if (!Solve(newModules, newSp))
                continue;
            double newHPWL = calculateHPWL(newModules);
            double delta = (newHPWL - HPWL) / HPWL;
            if (delta < 0)
            {
                modules = newModules;
                sp = newSp;
                HPWL = newHPWL;
                if (newHPWL < bestHPWL)
                {
                    bestModules = newModules;
                    bestHPWL = newHPWL;
                    bestSp = newSp;
                    bestModulesQueue.push(bestModules);
                    bestModulesQueue.pop();
                }
            }
            else
            {
                double rrr1 = exp(-delta * k / T);
                double rrr2 = (double)rand() / RAND_MAX;
                if (rrr2 < rrr1)
                {
                    modules = newModules;
                    sp = newSp;
                    HPWL = newHPWL;
                }
            }
        }
        T *= DOWN_T_RATE;
        //cout << ++c << ".Time: " << time(NULL) - startTime << " T: " << T << " Best_HPWL: " << bestHPWL << endl;
    }
    //modules = bestModules;
    //sp = bestSp;
    return bestModulesQueue;
}

queue<vector<Module> > MinHPWLSAWithSolveSA(vector<Module>& modules, SP& sp)
{
    double T = 100;
    const double k = ORIGIN_K;
    double HPWL = calculateHPWL(modules);

    double bestHPWL = HPWL;
    vector<Module> bestModules(modules);
    SP bestSp = sp;

    queue<vector<Module> > bestModulesQueue;
    for (int i = 0; i < 10; ++i)
        bestModulesQueue.push(modules);
    int c = 0;
    while (T > 0.05)
    //while (time(NULL) - startTime < TIME_LIMIT)
    {
        modules = bestModules;
        HPWL = bestHPWL;
        sp = bestSp;
        int initTime = time(NULL);
        while (++c < modulesCount * 10)
        //while (time(NULL) - initTime < TIMES_RATE && time(NULL) - startTime < TIME_LIMIT)
        {
            vector<Module> newModules = modules;
            SP newSp = sp;
            getNext(newModules, newSp);
            if (T >= 5)
            {
                if (!Solve(newModules, newSp))
                    continue;
            }
            else
            {
                if (!SolveSA(newModules, newSp, T))
                    continue;
            }
            double newHPWL = calculateHPWL(newModules);
            double delta = (newHPWL - HPWL) / HPWL;
            if (delta < 0)
            {
                modules = newModules;
                sp = newSp;
                HPWL = newHPWL;
                if (newHPWL < bestHPWL)
                {
                    bestModules = newModules;
                    bestHPWL = newHPWL;
                    bestSp = newSp;
                    bestModulesQueue.push(bestModules);
                    bestModulesQueue.pop();
                }
            }
            else
            {
                double rrr1 = exp(-delta * k / T);
                double rrr2 = (double)rand() / RAND_MAX;
                if (rrr2 < rrr1)
                {
                    modules = newModules;
                    sp = newSp;
                    HPWL = newHPWL;
                }
            }
        }
        T *= DOWN_T_RATE;
        //cout << ++c << ".Time: " << time(NULL) - startTime << " T: " << T << " Best_HPWL: " << bestHPWL << endl;
    }
    //modules = bestModules;
    //sp = bestSp;
    return bestModulesQueue;
}

bool Solve(vector<Module>& modules, SP& sp, int& area)
{
    SP_EVAL_ORIG(sp, modules);
    area = calculateOutChipArea(modules);
    for (int i = 0; i < softmodulesCount; ++i)
        area += getOverlapAreaWithFixedModules(modules[i], modules);

    if (area == 0)
        return true;
    else
        return false;
}

bool Solve(vector<Module>& modules, SP& sp)
{
    SP_EVAL_ORIG(sp, modules);
    if (isOutChip(modules))
        return false;

    for (int i = 0; i < softmodulesCount; ++i)
        if (isOverlapAreaWithFixedModules(modules[i], modules))
            return false;

    return true;
}

int getOverlapAreaWithFixedModules(const Module& m1, const vector<Module>& modules)
{
    int area = 0;
    for (int i = softmodulesCount; i < modulesCount; ++i)
    {
        Module m2 = modules[i];
        int overlapX = max(0, min(m1.x + m1.w, m2.x + m2.w) -
            max(m1.x, m2.x));
        int overlapY = max(0, min(m1.y + m1.h, m2.y + m2.h) -
            max(m1.y, m2.y));
        area += overlapX * overlapY;
    }
    return area;
}

bool isOverlapAreaWithFixedModules(const Module& m1, const vector<Module>& modules)
{
    for (int i = softmodulesCount; i < modulesCount; ++i)
    {
        Module m2 = modules[i];
        int overlapX = max(0, min(m1.x + m1.w, m2.x + m2.w) -
            max(m1.x, m2.x));
        int overlapY = max(0, min(m1.y + m1.h, m2.y + m2.h) -
            max(m1.y, m2.y));
        if (overlapX * overlapY > 0)
            return true;
    }
    return false;
}

void SP_EVAL_ORIG(const SP& sp, vector<Module>& modules)
{
    vector<int>width(modulesCount);
    vector<int>height(modulesCount);
    vector<int> positionsX(modulesCount);
    vector<int> positionsY(modulesCount);
    vector<int> XR(modulesCount);


    for (int i = 0; i < modulesCount; ++i)
    {
        width[i] = modules[i].w;
        height[i] = modules[i].h;
        XR[i] = sp.first[modulesCount - 1 - i];
    }

    //Calculate the x-coordinate of the module
    LCS_ORIG(sp.first, sp.second, positionsX, width);

    //Calculate the y-coordinate of the module
    LCS_ORIG(XR, sp.second, positionsY, height);

    for (int i = 0; i < softmodulesCount; ++i)
    {
        if (Left)
            modules[i].x = positionsX[i];
        else
            modules[i].x = chipW - positionsX[i] - modules[i].w;
        if (Down)
            modules[i].y = positionsY[i];
        else
            modules[i].y = chipH - positionsY[i] - modules[i].h;
    }
    return;
}

void LCS_ORIG(const vector<int>& X, const  vector<int>& Y, vector<int>& positions, const vector<int>& weights)
{
    vector<pair<int, int> > matchs(weights.size());
    vector<int> length(weights.size(), 0);


    for (int i = 0; i < X.size(); ++i)
    {
        matchs[X[i]].first = i;
        matchs[Y[i]].second = i;
    }

    for (int i = 0; i < X.size(); ++i)
    {
        int b = X[i];
        int p = matchs[b].second;
        positions[b] = length[p];
        int t = positions[b] + weights[b];
        for (int j = p; j < X.size(); j++)
        {
            if (t > length[j])
                length[j] = t;
            else
                break;
        }
    }
    return;
}

void initWH(const int& area, int& w, int& h)
{
    int d = (int)sqrt(area);

    double q = area / d + 1;
    if (area % d == 0)
        q = area / d;

    w = d;
    h = (int)q;
}

void reshapeModule(Module& m, const double& aspectRatio)
{
    int area = m.w * m.h;
    double width = sqrt(area * aspectRatio);
    double height = width / aspectRatio;

    // Set width and height as integers
    m.w = static_cast<int>(width);
    m.h = static_cast<int>(height);

    while (m.w * m.h < m.minArea)
    {
        ++m.w;
        ++m.h;
    }
}

bool readFile(vector<Module>& modules)
{
    ifstream fin(INPUT_FILE_NAME.c_str());
    if (!fin.is_open())
    {
        return false; // EXIT_FAILURE
    }

    // Read chip size
    string head;
    int count;
    fin >> head;
    fin >> chipW >> chipH;

    // Read soft modules
    fin >> head;
    fin >> softmodulesCount;
    for (int i = 0; i < softmodulesCount; ++i)
    {
        Module m;
        fin >> m.name >> m.minArea;
        m.x = m.y = 0;
        m.id = i;
        m.isFixed = false;
        initWH(m.minArea, m.w, m.h);
        modules.emplace_back(m);
    }

    // Read fixed modules
    fin >> head;
    fin >> count;
    for (int i = 0; i < count; ++i)
    {
        Module m;
        fin >> m.name >> m.x >> m.y >> m.w >> m.h;
        m.id = modules.size();
        m.isFixed = true;
        modules.emplace_back(m);
    }

    modulesCount = modules.size();

    // Read connections
    int maxC = -1;
    fin >> head;
    fin >> count;
    connections.resize(modules.size(), vector<int>(modules.size(), 0));
    for (int i = 0; i < count; ++i)
    {
        string m1, m2;
        int c;
        fin >> m1 >> m2 >> c;
        maxC = max(maxC, c);
        int index1 = -1, index2 = -1;
        for (int j = 0; j < modules.size(); ++j)
        {
            if (modules[j].name == m1)
            {
                index1 = j;
            }
            if (modules[j].name == m2)
            {
                index2 = j;
            }
        }
        if (index1 < index2)
            connections[index1][index2] += c;
        else
            connections[index2][index1] += c;
    }

    fin.close();
    return true;
}

double calculateHPWL(const vector<Module>& modules)
{
    double total = 0;
    vector<pair<double, double> >centers;
    for (int i = 0; i < modulesCount; ++i)
    {
        centers.emplace_back(make_pair(modules[i].x + (double)modules[i].w / 2, modules[i].y +
            (double)modules[i].h / 2));
    }
    for (int i = 0; i < modulesCount; ++i)
    {
        for (int j = i + 1; j < modulesCount; ++j)
        {
            if (connections[i][j] > 0)
                total += (fabs(centers[i].first - centers[j].first) +
                    fabs(centers[i].second - centers[j].second)) * connections[i][j];
        }
    }
    return total;
}

double calculateOutChipArea(const vector<Module>& modules)
{
    double excessAreaSum = 0;
    for (int i = 0; i < softmodulesCount; ++i)
    {
        int overlapX = max(0, min(chipW, modules[i].x + modules[i].w) -
            max(0, modules[i].x));
        int overlapY = max(0, min(chipH, modules[i].y + modules[i].h) -
            max(0, modules[i].y));
        excessAreaSum += modules[i].w * modules[i].h - overlapX * overlapY;
    }
    return excessAreaSum;
}

double isOutChip(const vector<Module>& modules)
{
    for (int i = 0; i < softmodulesCount; ++i)
    {
        int overlapX = max(0, min(chipW, modules[i].x + modules[i].w) -
            max(0, modules[i].x));
        int overlapY = max(0, min(chipH, modules[i].y + modules[i].h) -
            max(0, modules[i].y));
        if (modules[i].w * modules[i].h - overlapX * overlapY > 0)
            return true;
    }
    return false;
}

void outputFile(const vector<Module>& modules)
{
    ofstream fout(OUTPUT_FILE_NAME.c_str());
    double HPWL = calculateHPWL(modules);
    fout << fixed << showpoint << setprecision(1) << "HPWL " << HPWL << endl;

    fout << "SOFTMODULE " << softmodulesCount << endl;
    for (int i = 0; i < modules.size(); ++i)
    {
        if (!modules[i].isFixed)
        {
            fout << modules[i].name << " " << 4 << endl;
            fout << modules[i].x << " " << modules[i].y << endl;
            fout << modules[i].x << " " << modules[i].y + modules[i].h << endl;
            fout << modules[i].x + modules[i].w << " " << modules[i].y + modules[i].h << endl;
            fout << modules[i].x + modules[i].w << " " << modules[i].y << endl;
        }
    }
    fout.close();
}

vector<Module> moveModules(queue<vector<Module> > modulesQueue)
{
    vector<vector<Module> > modulesVec;
    while (!modulesQueue.empty())
    {
        modulesVec.emplace_back(modulesQueue.front());
        modulesQueue.pop();
    }
    double bestHPWL = calculateHPWL(modulesVec[modulesVec.size() - 1]);
    vector<Module> bestModules = modulesVec[modulesVec.size() - 1];
    int idx = 0;
    for (int i = modulesVec.size() - 1; i >= 0; --i)
    {
        vector<Module> modules = modulesVec[i];
        for (int i = 0; i < softmodulesCount * 5; ++i)
        {
            if (time(NULL) - startTime >= 1795)
                return bestModules;
            int n = rand() % softmodulesCount;
            set<int> stopPointX;
            set<int> stopPointY;
            stopPointX.insert(0);
            stopPointY.insert(0);
            for (int i = 0; i < modulesCount; ++i)
            {
                stopPointX.insert(modules[i].x);
                stopPointX.insert(modules[i].x + modules[i].w);
                stopPointY.insert(modules[i].y);
                stopPointY.insert(modules[i].y + modules[i].h);
            }

            vector<pair<int, int> > allPos = getCanPutPos(stopPointX, stopPointY, modules[n], modules);
            if (allPos.size() == 0)
                continue;
            pair<int, int> pos = findBestPos(allPos, modules[n], modules);
            modules[n].x = pos.first;
            modules[n].y = pos.second;
        }
        double HPWL = calculateHPWL(modules);
        if (HPWL < bestHPWL)
        {
            bestHPWL = HPWL;
            bestModules = modules;
        }
    }
    return bestModules;
}

vector<pair<int, int> > getCanPutPos(const set<int>& stopPointX, const set<int>& stopPointY, Module module, const vector<Module> modules)
{
    vector<pair<int, int> > pos;
    for (set<int>::iterator y = stopPointY.begin(); y != stopPointY.end(); ++y)
    {
        for (set<int>::iterator x = stopPointX.begin(); x != stopPointX.end(); ++x)
        {
            module.x = *x;
            module.y = *y;
            if (!testOverlappingAll(modules, module) && !isSingleOutChipArea(module))
                pos.emplace_back(make_pair(*x, *y));
        }
    }
    return pos;
}

bool isSingleOutChipArea(const Module& m)
{
    int excessAreaSum = 0;
    int overlapX = max(0, min(chipW, m.x + m.w) -
        max(0, m.x));
    int overlapY = max(0, min(chipH, m.y + m.h) -
        max(0, m.y));
    if (m.w * m.h - overlapX * overlapY > 0)
        return true;
    return false;
}

bool testOverlappingAll(const vector<Module>& modules, const Module& m)
{
    for (int i = 0; i < modulesCount; ++i)
    {
        if (modules[i].id != m.id)
        {
            if (m.x + m.w <= modules[i].x || modules[i].x + modules[i].w <= m.x || m.y + m.h <= modules[i].y || modules[i].y + modules[i].h <= m.y)
                continue;
            return true;
        }
    }
    return false;
}

pair<int, int> findBestPos(const vector<pair<int, int> > pos, const Module& module, vector<Module> modules)
{
    int n = module.id;
    modules[n].x = pos[0].first;
    modules[n].y = pos[0].second;
    double x = findIdealPosX(modules[n], modules);
    moveModulesX(x, modules[n], modules);
    double y = findIdealPosY(modules[n], modules);
    moveModulesY(y, modules[n], modules);
    double bestHPWL = calculateHPWL(modules);
    pair<int, int> bestPos = make_pair(modules[n].x, modules[n].y);

    for (int i = 1; i < pos.size(); ++i)
    {
        modules[n].x = pos[i].first;
        modules[n].y = pos[i].second;

        moveModulesX(x, modules[n], modules);
        moveModulesY(y, modules[n], modules);
        double newHPWL = calculateHPWL(modules);
        if (newHPWL < bestHPWL)
        {
            bestHPWL = newHPWL;
            bestPos = make_pair(modules[n].x, modules[n].y);
        }
    }

    return bestPos;
}

bool comparePairs(const pair<int, int>& p1, const pair<int, int>& p2)
{
    return p1.first < p2.first;
}

// Return the ideal center position of the module on the x-axis.
double findIdealPosX(Module& module, vector<Module>& modules)
{
    vector<pair<double, int> > xWithConnection;
    xWithConnection.emplace_back(make_pair(0, 0));
    double idealPosX;
    int connSize = connections.size();
    for (int i = 0; i < connSize; ++i)
    {
        for (int j = i + 1; j < connSize; ++j)
        {
            if (i == module.id && connections[i][j] != 0)
            {
                Module m = modules[j];
                xWithConnection.emplace_back(make_pair(m.x + 0.5 * (double)m.w, connections[i][j]));
            }
            else if (j == module.id && connections[i][j] != 0)
            {
                Module m = modules[i];
                xWithConnection.emplace_back(make_pair(m.x + 0.5 * (double)m.w, connections[i][j]));
            }
        }
    }

    // no connections with current module
    if (xWithConnection.size() == 0)
        return module.x;

    xWithConnection.emplace_back(make_pair(INT_MAX, 0));
    sort(xWithConnection.begin(), xWithConnection.end(), comparePairs);

    vector<double> x;
    int xConnSize = xWithConnection.size();
    for (int i = 1; i < xConnSize; ++i)
    {
        double coefficientOfX = 0;
        double coefficientOfContant = 0;
        for (int j = 1; j < xConnSize - 1; ++j)
        {
            if (xWithConnection[i].first <= xWithConnection[j].first)
            {
                coefficientOfX += -1 * xWithConnection[j].second;
                coefficientOfContant += xWithConnection[j].first * xWithConnection[j].second;
            }
            else
            {
                coefficientOfX += xWithConnection[j].second;
                coefficientOfContant += -1 * xWithConnection[j].first * xWithConnection[j].second;
            }
        }

        double value = -coefficientOfContant / coefficientOfX;
        if (value > xWithConnection[i].first)
        {
            value = xWithConnection[i].first - 0.5;
            if (module.w % 2 == 0)
            {
                if (value != int(value))
                    value -= 0.5;
            }
            else
            {
                if (value == int(value))
                    value -= 0.5;
            }
        }
        if (value < xWithConnection[i - 1].first)
        {
            value = xWithConnection[i - 1].first;
            if (int(module.w) % 2 == 0)
            {
                if (value != int(value))
                    value += 0.5;
            }
            else
            {
                if (value == int(value))
                    value += 0.5;
            }
        }
        if (int(module.w) % 2 == 0)
        {
            if (value != int(value))
                value += 0.5;
        }
        else
        {
            if (value == int(value))
                value += 0.5;
        }
        x.emplace_back(value);
    }

    double min = INT_MAX;
    double idx = 0;

    for (int i = 0; i < x.size(); ++i)
    {
        double value = 0;
        for (int j = 0; j < xConnSize; ++j)
            value += xWithConnection[j].second * fabs(x[i] - xWithConnection[j].first);
        if (value < min)
        {
            min = value;
            idx = i;
        }
    }

    if (x[idx] < (double)module.w / 2)
        x[idx] = (double)module.w / 2;
    else if (x[idx] > chipW - (double)module.w / 2)
        x[idx] = chipW - (double)module.w / 2;

    return x[idx];
}

// Return the ideal center position of the module on the y-axis.
double findIdealPosY(Module& module, vector<Module>& modules)
{
    vector<pair<double, int> > yWithConnection;
    yWithConnection.emplace_back(make_pair(0, 0));
    double idealPosY;
    int connSize = connections.size();
    for (int i = 0; i < connSize; ++i)
    {
        for (int j = i + 1; j < connSize; ++j)
        {
            if (i == module.id && connections[i][j] != 0)
            {
                Module m = modules[j];
                yWithConnection.emplace_back(make_pair(m.y + 0.5 * (double)m.h, connections[i][j]));
            }
            else if (j == module.id && connections[i][j] != 0)
            {
                Module m = modules[i];
                yWithConnection.emplace_back(make_pair(m.y + 0.5 * (double)m.h, connections[i][j]));
            }
        }
    }

    yWithConnection.emplace_back(make_pair(INT_MAX, 0));

    // no connections with current module
    if (yWithConnection.size() == 0)
        return module.x;

    sort(yWithConnection.begin(), yWithConnection.end(), comparePairs);

    vector<double> y;
    int yConnSize = yWithConnection.size();
    for (int i = 1; i < yConnSize; ++i)
    {
        double coefficientOfY = 0;
        double coefficientOfContant = 0;
        for (int j = 1; j < yConnSize - 1; ++j)
        {
            if (yWithConnection[i].first <= yWithConnection[j].first)
            {
                coefficientOfY += -1 * yWithConnection[j].second;
                coefficientOfContant += yWithConnection[j].first * yWithConnection[j].second;
            }
            else
            {
                coefficientOfY += yWithConnection[j].second;
                coefficientOfContant += -1 * yWithConnection[j].first * yWithConnection[j].second;
            }
        }

        double value = -coefficientOfContant / coefficientOfY;
        if (value > yWithConnection[i].first)
        {
            value = yWithConnection[i].first - 0.5;
            if (module.h % 2 == 0)
            {
                if (value != int(value))
                    value -= 0.5;
            }
            else
            {
                if (value == int(value))
                    value -= 0.5;
            }
        }
        if (value < yWithConnection[i - 1].first)
        {
            value = yWithConnection[i - 1].first;
            if (int(module.h) % 2 == 0)
            {
                if (value != int(value))
                    value += 0.5;
            }
            else
            {
                if (value == int(value))
                    value += 0.5;
            }
        }

        y.emplace_back(value);
    }

    double min = INT_MAX;
    double idx = 0;

    for (int i = 0; i < y.size(); ++i)
    {
        double value = 0;
        for (int j = 0; j < yConnSize; ++j)
            value += yWithConnection[j].second * fabs(y[i] - yWithConnection[j].first);
        if (value < min)
        {
            min = value;
            idx = i;
        }
    }

    if (y[idx] < (double)module.h / 2)
        y[idx] = (double)module.h / 2;
    else if (y[idx] > chipH - (double)module.h / 2)
        y[idx] = chipH - (double)module.h / 2;

    return y[idx];
}

// boundary[0] = up boundary
// boundary[1] = down boundary
// boundary[2] = left boundary
// boundary[3] = right boundary
vector<int> getModuleBoundary(const Module& module, const vector<Module>& modules)
{
    vector<int> boundary = { chipH ,0,0,chipW };

    for (int i = 0; i < modules.size(); ++i)
    {
        if (modules[i].id != module.id)
        {
            if (modules[i].y >= module.y + module.h && modules[i].y < boundary[0])
                boundary[0] = modules[i].y;
            if (modules[i].y + modules[i].h <= module.y && modules[i].y + modules[i].h > boundary[1])
                boundary[1] = modules[i].y + modules[i].h;
            if (modules[i].x + modules[i].w <= module.x && modules[i].x + modules[i].w > boundary[2])
                boundary[2] = modules[i].x + modules[i].w;
            if (modules[i].x >= module.x + module.w && modules[i].x < boundary[3])
                boundary[3] = modules[i].x;
        }
    }
    return boundary;
}

void moveModulesY(double y, Module& module, vector<Module>& modules)
{
    vector<int> boundary = getModuleBoundary(module, modules);
    y -= (double)module.h / 2;
    if (y + module.h <= boundary[0] && y >= boundary[1])
        module.y = y;
    else if (y + module.h > boundary[0])
        module.y = boundary[0] - module.h;
    else if (y < boundary[1])
        module.y = boundary[1];
}

void moveModulesX(double x, Module& module, vector<Module>& modules)
{
    vector<int> boundary = getModuleBoundary(module, modules);
    x -= (double)module.w / 2;
    if (x >= boundary[2] && x <= boundary[3] - module.w)
        module.x = x;
    else if (x < boundary[2])
        module.x = boundary[2];
    else if (x + module.w > boundary[3])
        module.x = boundary[3] - module.w;
}

bool readOutputFile(double& HPWL)
{
    ifstream file(OUTPUT_FILE_NAME.c_str());
    if (!file.is_open())
        return false;
    string s;
    file >> s;
    file >> HPWL;
    file.close();
    return true;
}