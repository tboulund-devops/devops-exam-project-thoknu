import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 5,
    duration: '10s',
};

export default () => {
    const baseUrl = __ENV.BASE_URL;
    
    const res = http.get(`${baseUrl}/api/tasks`);
    
    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });
    
    sleep(1);
}