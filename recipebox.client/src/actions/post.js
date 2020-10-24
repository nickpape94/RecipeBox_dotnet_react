import axios from 'axios';
import { setAlert } from './alert';
import {
	GET_POSTS,
	POST_ERROR,
	GET_POST,
	DELETE_POST,
	POST_SUBMIT_SUCCESS,
	POST_SUBMIT_FAIL,
	GET_PAGINATION_HEADERS,
	GET_PROFILE_PAGINATION_HEADERS
} from './types';

// Get all posts
export const getPosts = ({ pageNumber, setLoadingPage, searchParams, orderBy, userId }) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify({ searchParams, orderBy, userId });

	try {
		setLoadingPage(true);

		const res = await axios.post(`/api/posts?pageNumber=${pageNumber}`, body, config);

		const resHeaders = JSON.parse(res.headers.pagination);
		// console.log(resHeaders.currentPage);
		// console.log(resHeaders);

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
			type: GET_POSTS,
			payload: sortData
		});

		if (userId === '') {
			dispatch({
				type: GET_PAGINATION_HEADERS,
				payload: resHeaders
			});
		}

		if (userId.length > 0) {
			dispatch({
				type: GET_PROFILE_PAGINATION_HEADERS,
				payload: resHeaders
			});
		}

		setLoadingPage(false);
	} catch (err) {
		console.log(err);

		// dispatch({
		// 	type: POST_ERROR,
		// 	payload: { msg: err.response.statusText, status: err.response.status }
		// });
	}
};

// Get a post
export const getPost = (postId, setLoadingPage) => async (dispatch) => {
	try {
		setLoadingPage(true);

		const res = await axios.get(`/api/posts/${postId}`);

		dispatch({
			type: GET_POST,
			payload: res.data
		});

		setLoadingPage(false);
	} catch (err) {
		console.log(err);
		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Create a post
export const createPost = (
	userId,
	history,
	{ nameOfDish, description, ingredients, method, prepTime, cookingTime, feeds, cuisine }
) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	const body = JSON.stringify({
		nameOfDish,
		description,
		ingredients,
		method,
		prepTime,
		cookingTime,
		feeds,
		cuisine
	});

	try {
		const res = await axios.post(`/api/users/${userId}/posts`, body, config);

		dispatch({
			type: POST_SUBMIT_SUCCESS,
			payload: res.data
		});

		// history.push(`/add-photos/post/${res.data.postId}`);
		history.push('post/add-photos');
	} catch (err) {
		// console.log(err);
		const errors = err.response.data.errors;

		if (errors) {
			Object.keys(errors).forEach((key) => {
				dispatch(setAlert(errors[key], 'danger', 7000));
			});
		}

		dispatch({
			type: POST_SUBMIT_FAIL
		});
	}
};

// Delete post
export const deletePost = (id, postId, history, userId) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		await axios.delete(`/api/users/${id}/posts/${postId}`, config);

		dispatch({
			type: DELETE_POST,
			payload: postId
		});

		// console.log(history.location.pathname);
		// console.log(`/posts/${postId}`);

		// if (history.location.pathname === `/posts/${postId}`) {
		// 	console.log('true!');
		// 	history.push('/posts');
		// }

		// if (userId.length > 0) {
		// 	history.push(`/users/${userId}/posts`);
		// }

		history.push('/posts');
	} catch (err) {
		console.log(err);

		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
