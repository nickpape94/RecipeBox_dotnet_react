import axios from 'axios';
import {
	GET_USER_FAVOURITES,
	GET_USER_FAVOURITE,
	GET_PROFILE_PAGINATION_HEADERS,
	USER_ERROR,
	POST_ERROR,
	NOT_FAVOURITED,
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

		const sortData = res.data.map((post) => ({
			postId: post.postId,
			nameOfDish: post.nameOfDish,
			description: post.description,
			prepTime: post.prepTime,
			cookingTime: post.cookingTime,
			averageRating: post.averageRating,
			cuisine: post.cuisine,
			created: post.created,
			ratings: post.ratings,
			feeds: post.feeds,
			userId: post.userId,
			author: post.author,
			userPhotoUrl: post.userPhotoUrl,
			mainPhoto: post.postPhoto.filter((photo) => photo.isMain)[0]
		}));

		dispatch({
			type: GET_USER_FAVOURITES,
			payload: sortData
		});

		dispatch({
			type: GET_PROFILE_PAGINATION_HEADERS,
			payload: {
				resHeaders: resHeaders,
				fromPosts: false
			}
		});

		setLoadingPage(false);
	} catch (err) {
		dispatch({
			type: USER_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Get favourite
export const getFavourite = (userId, postId, setFavourited) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		const res = await axios.get(`/api/favourites/userId/${userId}/postId/${postId}`, config);

		dispatch({
			type: GET_USER_FAVOURITE,
			payload: res.data
		});

		setFavourited(true);
	} catch (err) {
		dispatch({
			type: NOT_FAVOURITED,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Add post to favourites
export const addToFavourites = (userId, postId, setFavourited) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		const res = await axios.post(`/api/favourites/userId/${userId}/postId/${postId}`, null, config);

		dispatch({
			type: ADD_POST_TO_FAVOURITES,
			payload: res.data
		});

		setFavourited(true);
	} catch (err) {
		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Delete post from favourites
export const deleteFavourite = (userId, postId, setFavourited) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		await axios.delete(`/api/favourites/userId/${userId}/postId/${postId}`, config);

		dispatch({
			type: DELETE_POST_FROM_FAVOURITES,
			payload: postId
		});

		if (setFavourited !== undefined) {
			setFavourited(false);
		}
	} catch (err) {
		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
