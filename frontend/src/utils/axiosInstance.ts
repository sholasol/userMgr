import axios from 'axios';
import { HOST_API_KEY } from '../globalConfig';

const axiosInstance = axios.create({baseURL: HOST_API_KEY });

//interceptor

axiosInstance.interceptors.response.use(
    (response) => response,
    (error)  => Promise.reject(
        (error.response && error.response) || 'General Axios error happened'
    )
)

export default axiosInstance;