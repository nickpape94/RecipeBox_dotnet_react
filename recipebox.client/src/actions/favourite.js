import axios from 'axios';
import { GET_USER_FAVOURITES, GET_PAGINATION_HEADERS, USER_ERROR } from './types';

// Get user favourites
export const getFavourites = ({ userId, pageNumber, setLoadingPage, orderBy }) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify({ orderBy });

	try {
		setLoadingPage(true);

		const res = await axios.post(`/api/favourites/userId/${userId}?pageNumber=${pageNumber}`, body, config);

		const resHeaders = JSON.parse(res.headers.pagination);

		dispatch({
			type: GET_USER_FAVOURITES,
			payload: res.data
		});

		dispatch({
			type: GET_PAGINATION_HEADERS,
			payload: resHeaders
		});

		setLoadingPage(false);
	} catch (err) {
		dispatch({
			type: USER_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
