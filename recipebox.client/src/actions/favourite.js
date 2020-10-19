import axios from 'axios';
import {
	GET_USER_FAVOURITES,
	GET_PROFILE_PAGINATION_HEADERS,
	USER_ERROR,
	POST_ERROR,
	ADD_POST_TO_FAVOURITES,
	DELETE_POST_FROM_FAVOURITES
} from './types';

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
			type: GET_PROFILE_PAGINATION_HEADERS,
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

// Add post to favourites
export const addToFavourites = ({ userId, postId }) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	try {
		const res = await axios.post(`/api/favourites/userId/${userId}/postId/${postId}`, config);

		dispatch({
			type: ADD_POST_TO_FAVOURITES,
			payload: res.data
		});
	} catch (err) {
		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Delete post from favourites
export const deleteFavourites = ({ userId, postId }) => async (dispatch) => {
	try {
		await axios.delete(`/api/favourites/userId/${userId}/postId/${postId}`);

		dispatch({
			type: DELETE_POST_FROM_FAVOURITES,
			payload: postId
		});
	} catch (err) {
		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
