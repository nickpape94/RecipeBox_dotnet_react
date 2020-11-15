import axios from 'axios';
import { setAlert } from './alert';
import {
	GET_POSTS,
	POST_ERROR,
	GET_POST,
	GET_CUISINE,
	RESET_PAGINATION,
	DELETE_POST,
	POST_SUBMIT_SUCCESS,
	POST_SUBMIT_FAIL,
	POST_UPDATE_SUCCESS,
	POST_UPDATE_FAIL,
	GET_PAGINATION_HEADERS,
	GET_PROFILE_PAGINATION_HEADERS,
	COMMENT_ADDED,
	COMMENT_UPDATED,
	COMMENT_REMOVED
} from './types';

// Get all posts
export const getPosts = ({ pageNumber, setLoadingPage, searchParams, orderBy, userId, match }) => async (dispatch) => {
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
		const currentLocation = window.location.href.split('/')[window.location.href.split('/').length - 1];
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
			payload: {
				postsToReturn: sortData,
				searchParams: currentLocation !== 'posts' ? '' : searchParams,
				orderBy: currentLocation !== 'posts' ? '' : orderBy
			}
		});

		// if ( window.location.href.split('/')[window.location.href.split('/').length - 1])

		console.log(window.location.href.split('/')[window.location.href.split('/').length - 1]);

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

		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Get posts by cuisine
// export const getPostsByCuisine = (cuisine) => async (dispatch) => {
// 	try {
// 		console.log(cuisine);
// 		const res = await axios.get(`/api/cuisine/${cuisine}`);

// 		dispatch({
// 			type: GET_CUISINE,
// 			payload: res.data
// 		});
// 	} catch (err) {
// 		dispatch({
// 			type: POST_ERROR,
// 			payload: { msg: err.response.statusText, status: err.response.status }
// 		});
// 	}
// };

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
		// console.log(err);
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
	setError,
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
		setError(true);

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

// Update post
export const updatePost = (
	userId,
	postId,
	setLoadingPage,
	setError,
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
		setLoadingPage(true);
		const res = await axios.put(`/api/users/${userId}/posts/${postId}`, body, config);

		dispatch({
			type: POST_UPDATE_SUCCESS,
			payload: res.data
		});

		setLoadingPage(false);
		history.push(`/posts/${postId}/edit/photos`);
	} catch (err) {
		setLoadingPage(false);
		setError(true);

		const errors = err.response.data.errors;

		if (errors) {
			Object.keys(errors).forEach((key) => {
				dispatch(setAlert(errors[key], 'danger', 7000));
			});
		}

		dispatch({
			type: POST_UPDATE_FAIL
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
		// console.log(err);

		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

export const addComment = (userId, postId, { comment: text }) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`,
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify({ text });

	// console.log(body);

	try {
		const res = await axios.post(`/api/users/${userId}/posts/${postId}/comments`, body, config);

		dispatch({
			type: COMMENT_ADDED,
			payload: res.data
		});
	} catch (err) {
		console.log(err);
	}
};

export const updateComment = ({ userId, postId, comment }) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`,
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify(comment);

	try {
		const res = await axios.put(`/api/users/${userId}/posts/${postId}/comments`, body, config);

		dispatch({
			type: COMMENT_UPDATED,
			payload: res.data
		});
	} catch (err) {
		console.log(err);
	}
};

export const deleteComment = (userId, commentId) => async (dispatch) => {
	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		await axios.delete(`/api/users/${userId}/comments/${commentId}`, config);

		dispatch({
			type: COMMENT_REMOVED,
			payload: commentId
		});
	} catch (err) {
		console.log(err);
	}
};

// Reset Pagination
export const resetPagination = () => async (dispatch) => {
	dispatch({
		type: RESET_PAGINATION
	});
};
