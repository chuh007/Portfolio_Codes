#pragma once
#include <vector>
//헤더만 있으므로 인클루드 해줘야함

inline Vec2 Lerf(const Vec2& p0, const Vec2& p1, double t)
{
	Vec2 p;
	p.x = (1.0 - t) * p0.x + t * p1.x;
	p.y = (1.0 - t) * p0.y + t * p1.y;
	return p;
}

inline Vec2 GetBezierPoint(const std::vector<Vec2>& _points, double t)
{
	int n = _points.size();

	if (n == 0)
		assert(n != 0 && "vector is null!!");
	else if(n==1)
		return _points[0];
	std::vector<Vec2> currentPoints = _points;

	for (int k = 1; k < n; ++k)
	{
		std::vector<Vec2> new_points;

		for (int i = 0; i < n - k; ++i)
		{
			Vec2 p0 = currentPoints[i];
			Vec2 p1 = currentPoints[i + 1];

			new_points.push_back(Lerf(p0, p1, t));
		}
		currentPoints = new_points;
	}

	return currentPoints[0];
}

struct ArcLengthEntry {
    double t;
    float s;
};

struct BezierPathData
{
    std::vector<Vec2> points;
    std::vector<ArcLengthEntry> arcLengthMap;
    float totalLength = 0.0f;

    inline void CalculateArcLengthMap(const std::vector<Vec2>& controlPoints, int samples = 500)
    {
        points = controlPoints;

        if (points.size() < 2) {
            arcLengthMap.clear();
            totalLength = 0.0f;
            return;
        }

        arcLengthMap.clear();
        totalLength = 0.0f;

        Vec2 p_prev = GetBezierPoint(points, 0.0); //이전샘플

        arcLengthMap.push_back({ 0.0, 0.0f });

        for (int i = 1; i <= samples; ++i)
        {
            double t = (double)i / (double)samples; //샘플의 해상도 (샘플간 간격)
            Vec2 p_current = GetBezierPoint(points, t); //현재 샘플

            float segment_dist = p_prev.Dist(p_current);
            totalLength += segment_dist;

            arcLengthMap.push_back({ t, totalLength });

            p_prev = p_current;
        }
    }

    inline double FindTValueForDistance(float target_s) const
    {
        if (arcLengthMap.empty()) return 0.0;
        if (target_s <= 0.0f) return 0.0;
        if (target_s >= totalLength) return 1.0;

        auto it = std::lower_bound(arcLengthMap.begin(), arcLengthMap.end(), target_s,
            [](const ArcLengthEntry& entry, float val) {
                return entry.s < val;
            });

        const auto& entry1 = *it;
        const auto& entry0 = *(it - 1); 

        double dist_range = entry1.s - entry0.s;
        double dist_offset = target_s - entry0.s;
        double alpha = dist_offset / dist_range; 

        return entry0.t * (1.0 - alpha) + entry1.t * alpha;
    }
};