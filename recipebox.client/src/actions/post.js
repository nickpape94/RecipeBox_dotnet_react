import axios from 'axios';
import { setAlert } from './alert';
import { GET_POSTS, POST_ERROR, GET_POST, POST_SUBMIT_SUCCESS, POST_SUBMIT_FAIL } from './types';

// Get posts
export const getPosts = () => async (dispatch) => {
	try {
		const res = await axios.get('/api/posts');

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
	} catch (err) {
		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Get a post
export const getPost = (postId) => async (dispatch) => {
	const res = await axios.get(`/api/posts/${postId}`);

	try {
		dispatch({
			type: GET_POST,
			payload: res.data
		});
	} catch (err) {
		console.log(err);
		dispatch({
			type: POST_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Submit a post
export const createPost = ({
	nameOfDish,
	description,
	ingredients,
	method,
	prepTime,
	cookingTime,
	feeds,
	cuisine,
	userId
}) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
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
		const res = await axios.post(`/api/users/${userId}/posts`);

		dispatch({
			type: POST_SUBMIT_SUCCESS,
			payload: res.data
		});
	} catch (err) {
		const errors = err.response.data;

		console.log(errors);

		if (errors) {
			errors.foreach((error) => dispatch(setAlert(error.description, 'danger')));
		}

		dispatch({
			type: POST_SUBMIT_FAIL
		});
	}
};
